using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoApi.Infrastructure;
using FomoAPI.Application.EventBuses;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    /// Used to store the results of the query from event bus to the database and cache.
    /// </summary>
    public class QueryResultStore : IQueryResultStore
    {
        private static object _lock;

        private IQueryCache _cache;

        public QueryResultStore(IQueryCache cache)
        {
            _cache = cache;
        }

        public void AddQuery(ISubscribableQuery query, ISubscriptionQueryResult result)
        {
            throw new NotImplementedException();
        }

        public ISubscriptionQueryResult GetQueryResult(ISubscribableQuery query)
        {
            throw new NotImplementedException();
        }

        public void RemoveQuery()
        {
            throw new NotImplementedException();
        }
    }
}
