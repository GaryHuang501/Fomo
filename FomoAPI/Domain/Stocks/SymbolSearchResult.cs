using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class SymbolSearchResult
    {
        public string Ticker { get; private set; }

        public string FullName { get; private set; }

        public decimal Match { get; private set; }

        [JsonConstructor]
        public SymbolSearchResult(string ticker, string fullName, decimal match)
        {
            Ticker = ticker;
            FullName = fullName;
            Match = match;
        }
    }
}
