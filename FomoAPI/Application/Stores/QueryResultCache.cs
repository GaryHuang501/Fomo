using FomoApi.Infrastructure;
using FomoAPI.Application.EventBuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    /// Stores 
    /// </summary>
    public class QueryResultCache : IQueryResultStore
    {
        private LoginContext _masterContext;

        private IQueryCache _cache;

        public void AddQuery(ISubscribableQuery query, ISubscriptionQueryResult result)
        {
            

        }

        public ISubscriptionQueryResult GetQueryResult (ISubscribableQuery query)
        {
            return null;
        }

        public void RemoveQuery()
        {

        }
    }
}
