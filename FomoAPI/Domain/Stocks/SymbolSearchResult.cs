using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Match Result for searching up a symbol.
    /// </summary>
    public record SymbolSearchResult
    {
        public string Ticker { get; init; }

        public string FullName { get; init; }

        public int Rank { get; init; }

        [JsonConstructor]
        public SymbolSearchResult(string ticker, string fullName, int rank)
        {
            Ticker = ticker;
            FullName = fullName;
            Rank = rank;
        }
    }
}
