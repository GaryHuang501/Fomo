using System;

namespace FomoAPI.Domain.Stocks.Queries
{
    public abstract class StockQueryResult
    {
        public string Symbol { get; private set; }

        public StockData Data { get; private set; }

        public DateTime CreateDateUtc { get; private set; }

        public string ErrorMessage { get; private set; }

        protected StockQueryResult(string symbol, StockData data)
        {
            Symbol = symbol;
            Data = data;
            CreateDateUtc = DateTime.UtcNow;
        }

        protected StockQueryResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool HasError => ErrorMessage != null;

    }
}
