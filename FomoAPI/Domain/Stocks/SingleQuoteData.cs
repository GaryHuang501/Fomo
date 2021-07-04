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
        public decimal ChangePercent { get; private set; }

        public decimal Price { get; private set; }

        public decimal Change { get; private set; }

        public string Ticker { get; private set; }

        public int SymbolId { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public SingleQuoteData(
            int symbolId,
            string ticker,
            decimal price,
            decimal change,
            decimal changePercent,
            DateTime lastUpdated)
        {
            SymbolId = symbolId;
            Ticker = ticker;
            Price = price;
            ChangePercent = changePercent;
            LastUpdated = lastUpdated;
        }
    }
}
