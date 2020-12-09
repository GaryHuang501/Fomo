using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    /// <summary>
    /// Class to sort querySubscription by subscriber count and filter out
    /// stale query results
    /// </summary>
    public class QuerySortBySubscriptionCountRule : IQueuePriorityRule
    {
        private readonly IQueryContextFactory _contextFactory;

        public QuerySortBySubscriptionCountRule(IQueryContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// Sort querySubscription by subscriber count and filter out stale query results
        /// </summary>
        /// <param name="querySubscriptions">QuerySubscription to sort</param>
        /// <returns>Sorted Queries</returns>
        public async Task<IEnumerable<StockQuery>> Sort(QuerySubscriptions querySubscriptions)
        {
            var subscriptionInfos = querySubscriptions.GetSubscriptionInfos();

            var subscriptionNeedRefreshing = await GetSubscriptionNeedRefreshing(querySubscriptions);
            
            var sortedQueriesBySubscriberCountAndStaleResultsDesc = subscriptionInfos.Where(s => s.HasSubscribers() && subscriptionNeedRefreshing.ContainsKey(s))
                                                                                 .OrderByDescending(s => s.SubscriberCount)
                                                                                 .Select(s => s.Query);

            return sortedQueriesBySubscriberCountAndStaleResultsDesc;
        }

        private async Task<Dictionary<SubscriptionInfo, StockQueryResult>> GetSubscriptionNeedRefreshing(QuerySubscriptions querySubscriptions)
        {
            var results = new Dictionary<SubscriptionInfo, StockQueryResult>();

            foreach(var info in querySubscriptions.GetSubscriptionInfos())
            {
                IQueryContext queryContext = info.Query.CreateContext(_contextFactory);

                StockQueryResult queryResult = await queryContext.GetQueryResult(info.Query.SymbolId);

                bool queryNeedsRefresh = queryResult == null || info.Query.FunctionType.IsExpired(queryResult.CreateDateUtc, DateTime.UtcNow);

                if (queryNeedsRefresh)
                {
                    results.Add(info, queryResult);
                }
            }

            return results;
        }
    }
}
