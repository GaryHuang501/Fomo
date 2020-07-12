using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Factory to create list of sync rules or changesets that be should be applied to existing symbol data.
    /// </summary>
    public interface IExchangeSyncChangesetsFactory
    {
        /// <summary>
        /// Creates the list of exchange sync changesets
        /// </summary>
        /// <param name="existingSymbolsMap">Readonly Dictionary for existing <see cref="Symbol"/> with <see cref="SymbolKey"/> as key.<</param>
        /// <param name="downloadedSymbolsMap">Readonly Dictionary for downloaded <see cref="DownloadedSymbol"/> with <see cref="SymbolKey"/> as key.</param>
        /// <returns>IEnumerable of <see cref="IExchangeSyncChangeset>"/></returns>
        public IEnumerable<IExchangeSyncChangeset> Create(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap,
                                                          IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap);
    }
}
