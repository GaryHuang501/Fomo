using System;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections.Generic;
using FomoAPI.Domain.Stocks.Queries;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Thread-safe Singleton class to tracks how many subscribers to a given query.
    /// This is used to determine which queries the event buses should run.
    /// </summary>
    public class QuerySubscriptions
    {
        /// <summary>
        /// Dictionary to map the query to the number of subscribers
        /// </summary>
        private readonly ConcurrentDictionary<StockQuery, SubscriptionInfo> _pendingQueriesMap;

        public int Count => _pendingQueriesMap.Keys.Count;

        public QuerySubscriptions()
        {
            _pendingQueriesMap = new ConcurrentDictionary<StockQuery, SubscriptionInfo>();
        }

        /// <summary>
        /// Add a subscriber to a query, incrementing the subscriber count
        /// </summary>
        /// <param name="query"></param>
        public void AddSubscriber(StockQuery query)
        {
            const int initialNumOfSubscribers = 1;

            _pendingQueriesMap.AddOrUpdate(query, new SubscriptionInfo(query, initialNumOfSubscribers), (key, oldValue) => new SubscriptionInfo(query, oldValue.SubscriberCount + 1));
        }

        /// <summary>
        /// Remove a subcriber for a query, reducing the subscriber count
        /// </summary>
        /// <param name="query"></param>
        public void RemoveSubscriber(StockQuery query)
        {
            _pendingQueriesMap.AddOrUpdate(query, new SubscriptionInfo(query, 0), (key, oldValue) => new SubscriptionInfo(key, Math.Max(0, oldValue.SubscriberCount - 1)));
        }

        /// <summary>
        /// Removes query from subscriptions, effectively setting subsriber count 0
        /// </summary>
        /// <param name="query"></param>
        public void ClearQuery(StockQuery query)
        {
            _pendingQueriesMap.TryRemove(query, out SubscriptionInfo subscriptionInfo);
        }

        public long GetSubscriberCount(StockQuery query)
        {
            if(_pendingQueriesMap.TryGetValue(query, out SubscriptionInfo subscription))
            {
                return subscription.SubscriberCount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get all the <see cref="SubscriptionInfo"/> 
        /// </summary>
        /// <returns>Returns all the <see cref="SubscriptionInfo"/> as a <see cref="IList"/></returns>
        /// <remarks>Using Enumerator to create the return list, improves performance by not locking. </remarks>
        public IList<SubscriptionInfo> GetSubscriptionInfos()
        {
            var queries = new List<SubscriptionInfo>();

            foreach(var kvp in _pendingQueriesMap)
            {
                queries.Add(kvp.Value);
            }

            return queries;
        }
    }
}
