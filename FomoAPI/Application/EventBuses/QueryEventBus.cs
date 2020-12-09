using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
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
        private readonly QueryPrioritySet _queryQueue;
        private readonly IQueryContextFactory _queryContextFactory;
        private readonly ILogger _logger;
        private readonly IQueuePriorityRule _queuePriorityRule;
        private readonly QuerySubscriptions _querySubscriptions;
        private readonly object _queryCounterLock;

        private int _maxQueryPerIntervalThreshold;

        /// <summary>
        /// Counter used to track how many queries left per interval.
        /// Should be reset after each interval.
        /// </summary>
        private int _intervalNumQueriesLeft;

        public QueryEventBus(QueryPrioritySet queryQueue,
                             IQueryContextFactory queryqueryContextRegistry,
                             IQueuePriorityRule queuePriorityRule,
                             ILogger<QueryEventBus> logger,
                             QuerySubscriptions querySubscriptions)
        {
            _queryQueue = queryQueue;
            _queryContextFactory = queryqueryContextRegistry;
            _logger = logger;
            _queryCounterLock = new object();
            _queuePriorityRule = queuePriorityRule;
            _querySubscriptions = querySubscriptions;
        }

        public void SetMaxQueryPerIntervalThreshold(int maxQueryPerIntervalThreshold)
        {
            _maxQueryPerIntervalThreshold = maxQueryPerIntervalThreshold;
        }

        public void ResetQueryExecutedCounter()
        {
            lock (_queryCounterLock)
            {
                _intervalNumQueriesLeft = _maxQueryPerIntervalThreshold;
            }
        }

        public async Task EnqueueNextQueries()
        {
            var prioritySortedQueries = (await _queuePriorityRule.Sort(_querySubscriptions)).ToList();
            int queryEnqueueCount = 0;

            foreach (var query in prioritySortedQueries)
            {
                bool isSuccess = _queryQueue.TryAdd(query);

                if (isSuccess)
                {
                    queryEnqueueCount++;
                }

                if (queryEnqueueCount >= _maxQueryPerIntervalThreshold)
                {
                    break;
                }
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

            var queriesToExecute = _queryQueue.Take(_maxQueryPerIntervalThreshold);

            _logger.LogInformation("{queryCount} queries pended up", queriesToExecute.Count());

            var queryTasks = queriesToExecute.Select(x => ExecuteQuery(x));
            await Task.WhenAll(queryTasks);
        }

        private async Task ExecuteQuery(StockQuery query)
        {
            try
            {
                lock (_queryCounterLock)
                {
                    if (_intervalNumQueriesLeft <= 0)
                    {
                        _logger.LogTrace("Query threshold for interval met. Exiting Execute Query.");
                        return;
                    }
                }

                _logger.LogTrace("Executing query for symbolId {symbol}", query.SymbolId);

                var queryContext = query.CreateContext(_queryContextFactory);

                await SaveQueryResult(queryContext, query);


                // Now that the query result has been updated in the store, remove it from the queue
                // so it can be requeued again when the data is stale
                _queryQueue.Remove(query);

                await ExecuteQueryResultTriggers(queryContext);

                _logger.LogTrace("Finished processing symbol query {SymbolId}", query.SymbolId);

            }
            catch (Exception ex)
            {
                _queryQueue.Remove(query);
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
