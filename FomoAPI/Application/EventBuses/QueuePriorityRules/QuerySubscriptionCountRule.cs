using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    /// <summary>
    /// Rule that prioritizes the queries to run by subscriber count.
    /// stale query results
    /// </summary>
    public class QuerySubscriptionCountRule : IQueuePriorityRule
    {
        private readonly IQueryContextFactory _contextFactory;

        private readonly QuerySubscriptions _querySubscriptions;

        private readonly ILogger<QuerySubscriptionCountRule> _logger;

        public QuerySubscriptionCountRule(IQueryContextFactory contextFactory, QuerySubscriptions querySubscriptions, ILogger<QuerySubscriptionCountRule> logger)
        {
            _contextFactory = contextFactory;
            _querySubscriptions = querySubscriptions;
            _logger = logger;
        }

        /// <summary>
        /// Get next set of queries needing to be updated, prioritized by subscriber count.
        /// </summary>
        /// <returns>Prioritized Queries as <see cref="IEnumerable{T}"/> of type <see cref="StockQuery"/></returns>
        public async Task<IEnumerable<StockQuery>> GetPrioritizedQueries()
        {
            _logger.LogTrace($"Begin Prioritizing Queries for {nameof(QuerySubscriptionCountRule)}");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var subscriptionInfos = _querySubscriptions.GetSubscriptionInfos();

            var subscriptionNeedRefreshing = await GetSubscriptionNeedRefreshing(subscriptionInfos);
            
            var sortedQueries = subscriptionInfos.Where(s => s.HasSubscribers() && subscriptionNeedRefreshing.ContainsKey(s))
                                                                                 .OrderByDescending(s => s.SubscriberCount)
                                                                                 .Select(s => s.Query);

            _logger.LogTrace($"{nameof(QuerySubscriptionCountRule)} took {stopWatch.ElapsedMilliseconds} ms to sort.");
            return sortedQueries;
        }

        public void ResetPriority(StockQuery query)
        {
            _querySubscriptions.ClearQuery(query);
        }

        private async Task<Dictionary<SubscriptionInfo, StockQueryResult>> GetSubscriptionNeedRefreshing(IList<SubscriptionInfo> subscriptions)
        {
            var results = new Dictionary<SubscriptionInfo, StockQueryResult>();

            foreach(var info in subscriptions)
            {
                IQueryContext queryContext = info.Query.CreateContext(_contextFactory);

                StockQueryResult queryResult = await queryContext.GetCachedQueryResult(info.Query.SymbolId);

                bool queryNeedsRefresh = queryResult == null || info.Query.FunctionType.IsExpired(queryResult.Data.LastUpdated, DateTime.UtcNow);

                if (queryNeedsRefresh)
                {
                    results.Add(info, queryResult);
                }
            }

            return results;
        }
    }
}
