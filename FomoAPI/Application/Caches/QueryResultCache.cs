using FomoAPI.Application.EventBuses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Caches
{
    public class QueryResultCache : IQueryResultStore
    {
       
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
