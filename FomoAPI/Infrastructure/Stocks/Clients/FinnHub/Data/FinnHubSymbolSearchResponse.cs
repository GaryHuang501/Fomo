using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks.Clients.FinnHub.Data
{
    /// <summary>
    /// POCO for FinnHub response when looking up matching symbols by keywords.
    /// </summary>
    public class FinnHubSymbolSearchResponse
    {
        public FinnHubSymbolMatch[] Result { get; set; }

        public int Count { get; set; }
    }
}
