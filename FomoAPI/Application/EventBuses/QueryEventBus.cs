using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-Safe Singleton class that will run the queries in the queue and save the results into a store (eg. cache)
    /// Scheduler should periodically reset a counter the keeps track how many queries should be run
    /// per interval. Alphavantage client only allows a certain amount of requests per minute.
    /// </summary>
    /// <inheritdoc cref="IQueryEventBus"></inheritdoc>/>
    public class QueryEventBus : IQueryEventBus
    {
        private readonly QueryQueue _queryQueue;
        private readonly IQueryContextFactory _queryContextFactory;
        private readonly ILogger _logger;
        private readonly IQueuePriorityRule _queuePriorityRule;

        private readonly SemaphoreSlim _queryEnqueueLock;

        private int _maxQueryPerIntervalThreshold;

        /// <summary>
        /// Counter used to track how many queries left per interval.
        /// Should be reset after each interval.
        /// </summary>
        private int _intervalNumQueriesLeft;

        public QueryEventBus(QueryQueue queryQueue,
                             IQueryContextFactory queryContextFactory,
                             IQueuePriorityRule queuePriorityRule,
                             ILogger<QueryEventBus> logger)
        {
            _queryQueue = queryQueue;
            _queryContextFactory = queryContextFactory;
            _logger = logger;
            _queryEnqueueLock = new SemaphoreSlim(1);
            _queuePriorityRule = queuePriorityRule;
        }

        public async Task SetMaxQueryPerIntervalThreshold(int maxQueryPerIntervalThreshold)
        {
            await _queryEnqueueLock.WaitAsync();

            try
            {
                _maxQueryPerIntervalThreshold = maxQueryPerIntervalThreshold;
            }
            finally
            {
                _queryEnqueueLock.Release();
            }
        }

        /// <summary>
        /// Reset back to starting state:
        /// 1) Query execution limit reset to how many queries left for this interval.
        /// 2) Queue is cleared.
        /// </summary>
        public async Task Reset()
        {
            // Should wait until items are dequeued so they are not lost.
            await _queryEnqueueLock.WaitAsync();
            try
            {
                int queriesRanCurrentInterval = _queryQueue.GetCurrentIntervalQueriesRanCount();

                _intervalNumQueriesLeft = _maxQueryPerIntervalThreshold - queriesRanCurrentInterval;

                _queryQueue.ClearAll();
            }
            finally
            {
                _queryEnqueueLock.Release();
            }
        }

        /// <summary>
        /// Executes the queries pending in QuerySubscriptions by priority.
        /// Then saves the data in the cache and runs any result triggers for against each query.
        /// </summary>
        /// <remarks> Function will exit when the max allowed query per Interval executed has been met</remarks>
        public async Task ExecutePendingQueries()
        {
            _logger.LogTrace("Fetching query priority list");

            // Query is removed from queue but the query cannot be enqueued again until 
            // the queue is cleared on the next refresh interval. This prevents a race condition
            // from happening where query would be execute mulitple times.

            await _queryEnqueueLock.WaitAsync();

            IList<StockQuery> queriesToExecute;

            try
            {
                await EnqueueNextQueries();
                queriesToExecute = _queryQueue.Dequeue(_maxQueryPerIntervalThreshold);
            }
            finally
            {
                _queryEnqueueLock.Release();
            }


            _logger.LogTrace("{queryCount} queries pended up", queriesToExecute.Count());

            var queryTasks = queriesToExecute.Select(async q => await ExecuteQuery(q));
            await Task.WhenAll(queryTasks);
        }

        /// <summary>
        /// Enqueue next prioritized queries to run.
        /// </summary>
        /// <remarks>Queue size should not exceed <see cref="_intervalNumQueriesLeft"/></remarks>
        private async Task EnqueueNextQueries()
        {
            if (_intervalNumQueriesLeft <= 0)
            {
                return;
            }

            var prioritySortedQueries = await _queuePriorityRule.GetPrioritizedQueries();

            foreach (var query in prioritySortedQueries)
            {
                bool success = _queryQueue.Enqueue(query);

                if (success)
                {
                    _intervalNumQueriesLeft--;
                    _queuePriorityRule.ResetPriority(query);
                }

                if (_intervalNumQueriesLeft <= 0)
                {
                    break;
                }
            }
        }

        private async Task ExecuteQuery(StockQuery query)
        {
            IQueryContext queryContext = null;

            try
            {
                _logger.LogTrace("Executing query for symbolId {symbol}", query.SymbolId);

                queryContext = query.CreateContext(_queryContextFactory);

                await SaveQueryResult(queryContext, query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error executing and saving query for symbol: {SymbolId}", query.SymbolId);
                return;
            }
            finally
            {
                _queryQueue.MarkAsExecuted(query);
            }

            try
            {
            _logger.LogTrace("Query for symbolId {symbol} cleared from queue", query.SymbolId);

                await ExecuteQueryResultTriggers(queryContext);
                await NotifyClients(queryContext, query);

                _logger.LogTrace("Finished processing symbol query {SymbolId}", query.SymbolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in event bus for query {SymbolId}", query.SymbolId);
            }
        }

        private async Task SaveQueryResult(IQueryContext queryContext, StockQuery query)
        {
            await queryContext.SaveQueryResultToStore();

            _logger.LogTrace("Saved query results for {symbol}", query.SymbolId);
        }

        private async Task ExecuteQueryResultTriggers(IQueryContext queryContext)
        {
            await queryContext.ExecuteResultTriggers();

            _logger.LogTrace("ExecutedQuery Result Triggers");
        }
        private async Task NotifyClients(IQueryContext queryContext, StockQuery query)
        {
            await queryContext.NotifyChangesClients();
            _logger.LogTrace("Notified Clients changes for {symbolId}", query.SymbolId);
        }
    }
}
