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
        /// <param name="existingSymbolsMap">Existing symbols as <see cref="IReadOnlyDictionary{SymbolKey, Symbol}"/>.</param>
        /// <param name="downloadedSymbolsMap">Downloaded symbols as <see cref="IReadOnlyDictionary{SymbolKey, Symbol}"/>.</param>
        /// <returns></returns>
        public IEnumerable<IExchangeSyncChangeset> Create(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, 
                                                          IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            return new List<IExchangeSyncChangeset>
                        {
                            new SymbolDetailsChangeset(existingSymbolsMap, downloadedSymbolsMap),
                            new NewSymbolChangeset(existingSymbolsMap, downloadedSymbolsMap),
                            new SymbolDelistChangeset(existingSymbolsMap, downloadedSymbolsMap),
                            new SymbolRelistChangeset(existingSymbolsMap, downloadedSymbolsMap),
                        };
        }
    }
}
