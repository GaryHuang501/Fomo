using Dapper;
using FomoAPI.Domain.Stocks.Queries;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Latest stock data for a given stock.
    /// </summary>
    public class SingleQuoteData : StockData, IEntity
    {
        public decimal High { get; private set; }

        public decimal Low { get; private set; }

        public decimal Open { get; private set; }

        public decimal PreviousClose { get; private set; }

        public decimal Change { get; private set; }

        public decimal ChangePercent { get; private set; }

        public decimal Price { get; private set; }

        public long Volume { get; private set; }

        public string Ticker { get; private set; }

        public int SymbolId { get; private set; }

        public DateTime LastTradingDay { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public SingleQuoteData(
            int symbolId,
            string ticker,
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
            SymbolId = symbolId;
            Ticker = ticker;
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
