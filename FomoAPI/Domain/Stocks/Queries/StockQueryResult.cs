using System;

namespace FomoAPI.Domain.Stocks.Queries
{
    public abstract record StockQueryResult
    {
        public string Ticker { get; private set; }

        public StockData Data { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public string ErrorMessage { get; private set; }

        public bool HasError { get; private set; }

        protected StockQueryResult(string ticker, StockData data, DateTime lastUpdated)
        {
            Ticker = ticker;
            Data = data;
            LastUpdated = lastUpdated;
        }

        protected StockQueryResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            HasError = true;
        }
    }
}
