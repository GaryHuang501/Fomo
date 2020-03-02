using FomoAPI.Application.Caches;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    /// <summary>
    /// Class to sort querySubscription by subscriber count and filter out
    /// stale query results
    /// </summary>
    public class QuerySortBySubscriptionCountRule : IQueuePriorityRule
    {
        private readonly IQueryResultStore _store;

        public QuerySortBySubscriptionCountRule(IQueryResultStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Sort querySubscription by subscriber count and filter out stale query results
        /// </summary>
        /// <param name="querySubscriptions">QuerySubscription to sort</param>
        /// <returns>Sorted Queries</returns>
        public IEnumerable<ISubscribableQuery> Sort(QuerySubscriptions querySubscriptions)
        {
            var subscriptionInfos = querySubscriptions.GetSubscriptionInfos();

            var sortedQueriesBySubscriberCountAndStaleResultsDesc = subscriptionInfos.Where(x => Filter(x, _store))
                                                                                 .OrderByDescending(x => x.SubscriberCount)
                                                                                 .Select(x => x.Query);

            return sortedQueriesBySubscriberCountAndStaleResultsDesc;
        }

        private bool Filter(SubscriptionInfo info, IQueryResultStore store)
        {
            return HasSubscribers(info)
                && IsDataStale(info.Query, store);
        }

        private bool HasSubscribers(SubscriptionInfo info) => info.SubscriberCount > 0;

        private bool IsDataStale(ISubscribableQuery query, IQueryResultStore store)
        {
            var queryResult = store.GetQueryResult(query);

            if(queryResult == null) return true;

            var isStaleData = query.FunctionType.IsExpired(queryResult.CreateDateUtc, DateTime.UtcNow);

            return isStaleData;
        }
    }
}
