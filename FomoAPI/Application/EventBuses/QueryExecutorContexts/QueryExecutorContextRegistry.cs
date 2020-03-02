using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueryExecutorContexts
{
    public class QueryExecutorContextRegistry : IQueryExecutorContextRegistry
    {
        private readonly AlphaVantageSingleQuoteQueryExecutorContext _alphaVantageSingleQuoteQueryExecutorContext;

        public QueryExecutorContextRegistry(AlphaVantageSingleQuoteQueryExecutorContext alphaVantageSingleQuoteQueryExecutorContext)
        {
            _alphaVantageSingleQuoteQueryExecutorContext = alphaVantageSingleQuoteQueryExecutorContext;
        }

        public IQueryExecutorContext<ISubscribableQuery, ISubscriptionQueryResult> GetExecutorContext(ISubscribableQuery query)
        {
            var context = _alphaVantageSingleQuoteQueryExecutorContext;

            return (IQueryExecutorContext<ISubscribableQuery, ISubscriptionQueryResult>)context;
        }
    }
}
