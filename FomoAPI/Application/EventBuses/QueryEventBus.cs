using FomoAPI.Application.EventBuses.QueryExecutorContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Application.EventBuses.Triggers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-Safe Singleton class that will run the queries in the queue and save the results into a store (eg. cache)
    /// Every scheduler run should reset the number of queries allowed per Interval
    /// since Alphavantage allows only an x Amount of api calls per Interval.
    /// </summary>
    public class QueryEventBus : IQueryEventBus
    {
        private readonly QueryPrioritySet _queryQueue;
        private readonly IQueryExecutorContextRegistry _queryExecutorContextRegistry;
        private readonly ILogger _logger;
        private readonly IQueuePriorityRule _queuePriorityRule;
        private readonly QuerySubscriptions _querySubscriptions;
        private readonly object _queriesExecutedCounterLock;

        private int _maxQueryPerIntervalThreshold;
        private int _queriesExecutedCounter;

        public QueryEventBus(QueryPrioritySet queryQueue,
                             IQueryExecutorContextRegistry queryExecutorContextRegistry,
                             IQueuePriorityRule queuePriorityRule,
                             ILogger<QueryEventBus> logger,
                             QuerySubscriptions querySubscriptions)
        {
            _queryQueue = queryQueue;
            _queryExecutorContextRegistry = queryExecutorContextRegistry;
            _logger = logger;
            _queriesExecutedCounterLock = new object();
            _queuePriorityRule = queuePriorityRule;
            _querySubscriptions = querySubscriptions;
        }

        /// <summary>
        /// Set the max query that can be run per Interval. 
        /// </summary>
        /// <remarks>If some queries get delayed from running on the previous interval, this will prevent too many
        /// queries from being run on the next in</remarks>
        /// <param name="maxQueryPerIntervalThreshold">Max number of queries per Interval allowed by the Data API</param>
        public void SetMaxQueryPerIntervalThreshold(int maxQueryPerIntervalThreshold)
        {
            _maxQueryPerIntervalThreshold = maxQueryPerIntervalThreshold;
        }

        public void ResetQueryExecutedCounter()
        {
            _queriesExecutedCounter = 0;
        }

        public void EnqueueNextQueries()
        {
            var prioritySortedQueries = _queuePriorityRule.Sort(_querySubscriptions).ToList();
            int queryEnqueueCount = 0;

            foreach(var query in prioritySortedQueries)
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
        /// Executes the queries in the QuerySubscriptions in priority 
        /// and saves the data in the cache and runs any result triggers for each query context.
        /// Function will exit when the allowed query per Interval executed threshold has been exceeded.
        /// </summary>
        /// <returns>Task to await</returns>
        public async Task ExecutePendingQueriesAsync()
        {
            _logger.LogTrace($"Fetching query priority list");

            var queriesToExecute = _queryQueue.Take(_maxQueryPerIntervalThreshold);

            _logger.LogInformation($"{queriesToExecute.Count()} queries pended up");

            var queryTasks = queriesToExecute.Select(x => ExecuteQuery(x));
            await Task.WhenAll(queryTasks);
        }

        private async Task ExecuteQuery(ISubscribableQuery query)
        {
            try
            {
                _logger.LogTrace($"Executing query {query.Symbol}");

                var executorContext = _queryExecutorContextRegistry.GetExecutorContext(query);

                // lock to prevent race condition where 2 queries get executed at same time
                // when the counter is one less than the maximum. 
                // This can happen if the scheduler schedules another run but the previous run's
                // task gets delayed and overlaps the current run.
                lock (_queriesExecutedCounterLock)
                {
                    _queriesExecutedCounter++;
                    if (_queriesExecutedCounter >= _maxQueryPerIntervalThreshold) return;
                }

                var queryResult = await FetchQueryResultAsync(executorContext, query);

                await executorContext.SaveToStoreAsync(query, queryResult);

                // Now that the query result has been updated in the store, remove it from the queue
                // so it can be requeued again when the data is stale
                _queryQueue.Remove(query);

                await ExecuteQueryResultTriggers(executorContext, queryResult);

                _logger.LogTrace($"Finished processing query {query.Symbol}");

            }
            catch (Exception ex)
            {
                _queryQueue.Remove(query);
                _logger.LogError(ex, $"Unexpected error in event bus for query {query.Symbol}");
            }
        }

        private async Task<ISubscriptionQueryResult> FetchQueryResultAsync(IQueryExecutorContext<ISubscribableQuery, ISubscriptionQueryResult> executorContext, ISubscribableQuery query)
        {
           var queryResult = await executorContext.FetchQueryResultAsync(query);

            _logger.LogTrace($"Saving query results {query.Symbol}");

            if (queryResult.HasError)
            {
                _logger.LogError($"Error running query {query.Symbol}: {queryResult.ErrorMessage}");
            }

            return queryResult;
        }

        private async Task ExecuteQueryResultTriggers(IQueryExecutorContext<ISubscribableQuery, ISubscriptionQueryResult> executorContext, ISubscriptionQueryResult result)
        {
            var queryResultTriggers = executorContext.GetQueryResultTriggers();

            _logger.LogTrace($"Executing Query Result Triggers");

            foreach (var trigger in queryResultTriggers)
            {
                await trigger.ExecuteAsync(result);
            }
        }
    }
}
