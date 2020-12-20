using Dapper;
using FomoAPI.Domain.Stocks.Queries;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Latest stock data for a given stock.
    /// </summary>
    public class StockSingleQuoteData : StockData, IEntity
    {
        public decimal High { get; private set; }

        public decimal Low { get; private set; }

        public decimal Open { get; private set; }

        public decimal PreviousClose { get; private set; }

        public decimal Change { get; private set; }

        public decimal ChangePercent { get; private set; }

        public decimal Price { get; private set; }

        public long Volume { get; private set; }

        public string Symbol { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public DateTime LastTradingDay { get; private set; }


        [JsonConstructor]
        [ExplicitConstructor]
        public StockSingleQuoteData(
            string symbol,
            decimal high,
            decimal low,
            decimal open,
            decimal price,
            decimal previousClose,
            long volume,
            decimal change,
            decimal changePercent,
            DateTime lastUpdated, 
            DateTime lastTradingDay)
        {
            Symbol = symbol;
            High = high;
            Low = low;
            Open = open;
            Price = price;
            Volume = volume;
            PreviousClose = previousClose;
            Change = change;
            ChangePercent = changePercent;
            LastUpdated = lastUpdated;
            LastTradingDay = lastTradingDay;
        }
    }
}
