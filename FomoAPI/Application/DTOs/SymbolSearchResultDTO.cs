using Dapper;
using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.DTOs
{
    /// <summary>
    /// Stock search result DTO returned to the client
    /// that has both the StockClient our own Domain IDs.
    /// </summary>
    public class SymbolSearchResultDTO
    {
        public bool Delisted { get; set; }

        public int ExchangeId { get; set; }

        public string ExchangeName { get; set; }

        public string FullName { get; private set; }

        public decimal Match { get; private set; }

        public int SymbolId { get; set; }

        public string Ticker { get; set; }

        public SymbolSearchResultDTO(SymbolSearchResult searchResult, Symbol symbol)
        {
            Delisted = symbol.Delisted;
            ExchangeId = symbol.ExchangeId;
            ExchangeName = symbol.ExchangeName;
            FullName = searchResult.FullName;
            Match = searchResult.Match;
            Ticker = searchResult.Ticker;
            SymbolId = symbol.Id;
        }

        [JsonConstructor]
        public SymbolSearchResultDTO(bool delisted, int exchangeId, string exchangeName, string fullName, decimal match, int symbolId, string ticker)
        {
            Delisted = delisted;
            ExchangeId = exchangeId;
            ExchangeName = exchangeName;
            FullName = fullName;
            Match = match;
            SymbolId = symbolId;
            Ticker = ticker;
        }
    }
}
