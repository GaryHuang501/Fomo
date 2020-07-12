using FomoAPI.Domain.Stocks;
using System.Collections.Generic;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Factory to create list of sync rules or changesets that be should be applied to existing symbol data.
    /// </summary>
    public class ExchangeSyncChangesetsFactory: IExchangeSyncChangesetsFactory
    {
        /// <summary>
        /// Creates the list of exchange sync changesets
        /// </summary>
        /// <param name="existingSymbolsMap">Readonly Dictionary for existing <see cref="Symbol"/> with <see cref="SymbolKey"/> as key.<</param>
        /// <param name="downloadedSymbolsMap">Readonly Dictionary for downloaded <see cref="DownloadedSymbol"/> with <see cref="SymbolKey"/> as key.</param>
        /// <returns></returns>
        public IEnumerable<IExchangeSyncChangeset> Create(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, 
                                                          IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            return new List<IExchangeSyncChangeset>
                        {
                            new SymbolDetailsChangeset(existingSymbolsMap, downloadedSymbolsMap),
                            new NewSymbolChangeset(existingSymbolsMap, downloadedSymbolsMap),
                            new SymbolDelistChangeset(existingSymbolsMap, downloadedSymbolsMap),
                        };
        }
    }
}
