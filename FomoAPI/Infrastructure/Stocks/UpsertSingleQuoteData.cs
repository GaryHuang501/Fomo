using FomoAPI.Domain.Stocks;
using System;

namespace FomoAPI.Infrastructure.Stocks
{
    /// <summary>
    /// Action to upsert single quote data to database.
    /// </summary>
    public class UpsertSingleQuoteData
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

        public int SymbolId { get; private set; }

        public UpsertSingleQuoteData(int symbolId, StockSingleQuoteData data)
        {
            SymbolId = symbolId;
            High = data.High;
            Low = data.Low;
            Open = data.Open;
            Price = data.Price;
            Volume = data.Volume;
            PreviousClose = data.PreviousClose;
            Change = data.Change;
            ChangePercent = data.ChangePercent;
            Symbol = data.Symbol;
            LastUpdated = data.LastUpdated;
            LastTradingDay = data.LastTradingDay;
        }
    }
}
