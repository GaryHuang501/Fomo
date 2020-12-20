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
                             IQueryContextFactory queryqueryContextRegistry,
                             IQueuePriorityRule queuePriorityRule,
                             ILogger<QueryEventBus> logger)
        {
            _queryQueue = queryQueue;
            _queryContextFactory = queryqueryContextRegistry;
            _logger = logger;
            _queryEnqueueLock = new SemaphoreSlim(1);
            _queuePriorityRule = queuePriorityRule;
        }

        public void SetMaxQueryPerIntervalThreshold(int maxQueryPerIntervalThreshold)
        {
            _maxQueryPerIntervalThreshold = maxQueryPerIntervalThreshold;
        }

        /// <summary>
        /// Reset back to starting state:
        /// 1) Query execution limit reset to how many queries left for this interval.
        /// 2) Queue is cleared.
        /// </summary>
        public async Task Reset()
        {
            await _queryEnqueueLock.WaitAsync();

            int queriesRanCurrentInterval = _queryQueue.GetCurrentIntervalQueriesRanCount();

            _intervalNumQueriesLeft = _maxQueryPerIntervalThreshold - queriesRanCurrentInterval;

            _queryQueue.ClearAll();
            _queryEnqueueLock.Release();
        }

        /// <summary>
        /// Executes the queries pending in QuerySubscriptions by priority.
        /// Then saves the data in the cache and runs any result triggers for against each query.
        /// </summary>
        /// <remarks> Function will exit when the max allowed query per Interval executed has been met</remarks>
        public async Task ExecutePendingQueries()
        {
            _logger.LogTrace("Fetching query priority list");

            // Query is removed from queue but the query will not be enqueued again until 
            // it is queries are cleared during the next refresh interval. This prevents a race condition
            // from happening where query would be execute mulitple times.
            await _queryEnqueueLock.WaitAsync();
            await EnqueueNextQueries();
            var queriesToExecute = _queryQueue.Dequeue(_maxQueryPerIntervalThreshold);
            _queryEnqueueLock.Release();

            _logger.LogInformation("{queryCount} queries pended up", queriesToExecute.Count());

            var queryTasks = queriesToExecute.Select(async q => await ExecuteQuery(q));
            await Task.WhenAll(queryTasks);
        }

        /// <summary>
        /// Enqueue next prioritized queries to run.
        /// </summary>
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
            try
            {
                _logger.LogTrace("Executing query for symbolId {symbol}", query.SymbolId);

                var queryContext = query.CreateContext(_queryContextFactory);

                await SaveQueryResult(queryContext, query);
                _queryQueue.MarkAsExecuted(query);

                _logger.LogTrace("Query for symbolId {symbol} cleared from queue", query.SymbolId);

                await ExecuteQueryResultTriggers(queryContext);

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

            _logger.LogTrace("Saving query results for {symbol}", query.SymbolId);
        }

        private async Task ExecuteQueryResultTriggers(IQueryContext queryContext)
        {
            await queryContext.ExecuteResultTriggers();

            _logger.LogTrace("Executing Query Result Triggers");
        }
    }
}
