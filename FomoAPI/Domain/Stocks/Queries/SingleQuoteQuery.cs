using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks.Queries
{
    /// <summary>
    /// Query to fetch <see cref="SingleQuoteData"/>for a stock.
    /// </summary>
    public record SingleQuoteQuery : StockQuery
    {
        public SingleQuoteQuery(int symbolId)
            : base(symbolId, QueryFunctionType.SingleQuote)
        {

        }

        public override IQueryContext CreateContext(IQueryContextFactory contextFactory)
        {
            return contextFactory.GenerateSingleQuoteContext(this);
        }
    }
}
