using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class SymbolSearchResult
    {
        public string Symbol { get; private set; }

        public string FullName { get; private set; }

        public decimal Match { get; private set; }

        public SymbolSearchResult(string symbol, string fullName, decimal match)
        {
            Symbol = symbol;
            FullName = fullName;
            Match = match;
        }
    }
}
