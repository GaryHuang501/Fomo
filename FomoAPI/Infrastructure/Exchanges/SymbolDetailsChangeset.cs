using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Changeset for any changes to symbols
    /// </summary>
    public class SymbolDetailsChangeset : IExchangeSyncChangeset
    {
        private List<UpdateSymbolAction> _changedSymbols;
        private IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public SymbolDetailsChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _changedSymbols = new List<UpdateSymbolAction>();
            _existingSymbolsMap = existingSymbolsMap;
            _downloadedSymbolsMap = downloadedSymbolsMap;
        }

        /// <summary>
        /// Add new Symbol to changeset to be inserted into database
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        public void AddChange(SymbolKey symbolKey)
        {
            var symbolForDb = new UpdateSymbolAction(
                                    _existingSymbolsMap[symbolKey].Id,
                                    _downloadedSymbolsMap[symbolKey].FullName,
                                    _downloadedSymbolsMap[symbolKey].ExchangeId);


            _changedSymbols.Add(symbolForDb);
        }

        /// <summary>
        /// Check if the given symbol is new or not.
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        /// <returns>True if symbol is new. Otherwise False.</returns>
        public bool HasChanges(SymbolKey symbolKey)
        {
            if(_existingSymbolsMap.TryGetValue(symbolKey, out Symbol existingSymbol) && 
               _downloadedSymbolsMap.TryGetValue(symbolKey, out DownloadedSymbol downloadedSymbol))
            {
                return existingSymbol.FullName != downloadedSymbol.FullName;
            }

            return false;
        }

        public ThresholdCheck GetThresholdCheck(ExchangeSyncSetting setting)
        {
            return new ThresholdCheck(
                nameof(SymbolDetailsChangeset),
                _changedSymbols.Count, 
                setting.UpdateThresholdPercent);
        }

        /// <summary>
        /// Saves all new symbols to database.
        /// </summary>
        /// <param name="repository"><see cref="IExchangeSyncRepository"/> to save to database.</param>
        public async Task SaveChangeset(IExchangeSyncRepository repository)
        {
            if (_changedSymbols.Count == 0) return;

            var symbolsUpdated = await repository.UpdateSymbols(_changedSymbols);

            await repository.AddSyncHistory(nameof(SymbolDetailsChangeset), symbolsUpdated, $"{symbolsUpdated} symbols updated");

        }
    }
}
