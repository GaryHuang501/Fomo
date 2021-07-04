using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks.Clients.FinnHub.Data
{
    /// <summary>
    /// POCO for each matching Symbol found
    /// </summary>
    public class FinnHubSymbolMatch
    {
        public string Description { get; set; }

        public string Symbol { get; set; }

        public string DisplaySymbol { get; set; }

        public string Type { get; set; }
    }
}
