using FomoAPI.Domain.Stocks;
using System;

namespace FomoAPI.Infrastructure.Stocks
{
    /// <summary>
    /// Action to upsert single quote data to database.
    /// </summary>
    public class UpsertSingleQuoteData
    {
        public decimal Change { get; private set; }

        public decimal ChangePercent { get; private set; }

        public decimal Price { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public int SymbolId { get; private set; }

        public UpsertSingleQuoteData(int symbolId, SingleQuoteData data)
        {
            SymbolId = symbolId;
            Price = data.Price;
            ChangePercent = data.ChangePercent;
            LastUpdated = data.LastUpdated;
        }
    }
}
