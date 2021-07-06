using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks.Queries
{
    public record SingleQuoteQueryResult : StockQueryResult
    {
        public new SingleQuoteData Data { get; private set; }

        public SingleQuoteQueryResult(string ticker, SingleQuoteData data) : base(ticker, data, data.LastUpdated)
        {
            Data = data;
        }

        public SingleQuoteQueryResult(string errorMessage) : base(errorMessage)
        {
        }
    }
}
