using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks.Queries
{
    public class SingleQuoteQueryResult : StockQueryResult
    {
        public new StockSingleQuoteData Data { get; private set; }

        public SingleQuoteQueryResult(string symbol, StockSingleQuoteData data) : base(symbol, data, data.LastUpdated)
        {
            Data = data;
        }

        public SingleQuoteQueryResult(string errorMessage) : base(errorMessage)
        {
        }
    }
}
