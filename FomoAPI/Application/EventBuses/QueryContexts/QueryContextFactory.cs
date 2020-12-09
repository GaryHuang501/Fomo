using FomoAPI.Domain.Stocks.Queries;
using System;

namespace FomoAPI.Application.EventBuses.QueryContexts
{
    public class QueryContextFactory : IQueryContextFactory
    {
        private readonly Func<SingleQuoteQuery, SingleQuoteContext> _singleQuoteFactory;

        public QueryContextFactory(Func<SingleQuoteQuery, SingleQuoteContext> singleQuoteFactory)
        {
            _singleQuoteFactory = singleQuoteFactory;
        }
        
        public SingleQuoteContext GenerateSingleQuoteContext(SingleQuoteQuery query)
        {
            return _singleQuoteFactory(query);
        }
    }
}
