using Dapper;
using FomoAPI.Domain.Stocks.Queries;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Latest stock data for a given stock.
    /// </summary>
    public record SingleQuoteData : StockData, IEntity
    {
        public decimal ChangePercent { get; init; }

        public decimal Price { get; init; }

        public decimal Change { get; init; }

        public string Ticker { get; init; }

        public int SymbolId { get; init; }

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
