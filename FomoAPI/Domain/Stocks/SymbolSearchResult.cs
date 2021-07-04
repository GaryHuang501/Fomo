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
    public class SymbolSearchResult
    {
        public string Ticker { get; private set; }

        public string FullName { get; private set; }

        public int Rank { get; private set; }

        [JsonConstructor]
        public SymbolSearchResult(string ticker, string fullName, int rank)
        {
            Ticker = ticker;
            FullName = fullName;
            Rank = rank;
        }
    }
}
