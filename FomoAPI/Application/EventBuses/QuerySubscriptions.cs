using System;
using FomoAPI.Infrastructure.AlphaVantage;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using FomoAPI.Application.Caches;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-safe class to tracks how many subscribers to a given query of type T.
    /// Used to determine which queries the event buses should run.
    /// </summary>
    public class QuerySubscriptions
    {
        /// <summary>
        /// Dictionary to map the query to the number of subscribers
        /// </summary>
        private readonly ConcurrentDictionary<ISubscribableQuery, SubscriptionInfo> _pendingQueriesMap;

        public int Count => _pendingQueriesMap.Keys.Count;

        public QuerySubscriptions()
        {
            _pendingQueriesMap = new ConcurrentDictionary<ISubscribableQuery, SubscriptionInfo>();
        }

        /// <summary>
        /// Add a subscriber to a query, incrementing the subscriber count
        /// </summary>
        /// <param name="query"></param>
        public void AddSubscriber(ISubscribableQuery query)
        {
            const int initialNumOfSubscribers = 1;

            _pendingQueriesMap.AddOrUpdate(query, new SubscriptionInfo(query, initialNumOfSubscribers), (key, oldValue) => new SubscriptionInfo(key, oldValue.SubscriberCount + 1));
        }

        /// <summary>
        /// Remove a subcriber for a query, reducing the subscriber count
        /// </summary>
        /// <param name="query"></param>
        public void RemoveSubscriber(ISubscribableQuery query)
        {
            _pendingQueriesMap.AddOrUpdate(query, new SubscriptionInfo(query, 0), (key, oldValue) => new SubscriptionInfo(key, Math.Max(0, oldValue.SubscriberCount - 1)));
        }

        /// <summary>
        /// Removes query from subscriptions, effectively setting subsriber count 0
        /// </summary>
        /// <param name="query"></param>
        public void ClearQuery(ISubscribableQuery query)
        {
            _pendingQueriesMap.TryRemove(query, out SubscriptionInfo subscriptionInfo);
        }

        public IEnumerable<SubscriptionInfo> GetSubscriptionInfos(){
            return _pendingQueriesMap.Values.ToList();
        }
    }
}
