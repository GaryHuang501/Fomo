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
        /// Get all the listed traded symbols for for all major stock exchanges
        /// </summary>
        /// <param name="syncSettings">The <see cref="ExchangeSyncSetting"/> containing sync settings.</param>
        /// <returns>IReadOnlyDictionary with <see cref="SymbolKey"/> as key and <see cref="DownloadedSymbol"/> for the value.</returns>
        Task<IReadOnlyDictionary<SymbolKey, DownloadedSymbol>> GetTradedSymbols(ExchangeSyncSetting syncSettings);
    }
}
