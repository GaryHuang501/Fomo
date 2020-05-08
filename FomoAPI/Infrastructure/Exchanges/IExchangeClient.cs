using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Client to fetch traded symbol data from an exchange
    /// </summary>
    public interface IExchangeClient
    {
        /// <summary>
        /// Get all the listed traded symbols for for stock exchanges
        /// </summary>
        /// <returns>Dictionary with Ticker Symbol as key and the downloaded symbol as the value.</returns>
        public Task<IDictionary<string, DownloadedSymbol>> GetTradedSymbols();
    }
}
