using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class PortfolioSymbol
    {
        public int Id { get; set; }

        public int SymbolId { get; set; }

        public string Ticker { get; set; }

        public string ExchangeName { get; set; }

        public string FullName { get; set; }

        public bool Delisted { get; set; }

        public int SortOrder { get; set; }
    }
}
