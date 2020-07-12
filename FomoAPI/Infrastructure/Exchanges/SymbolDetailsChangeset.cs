using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class SymbolDetailsChangeset : IExchangeSyncChangeset
    {
        private List<Symbol> _changedSymbols;
        private IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public SymbolDetailsChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _changedSymbols = new List<Symbol>();
            _existingSymbolsMap = existingSymbolsMap;
            _downloadedSymbolsMap = downloadedSymbolsMap;
        }

        /// <summary>
        /// Add new Symbol to changeset to be inserted into database
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        public void AddChange(SymbolKey symbolKey)
        {
            var symbolForDb = new Symbol
            {
                Id = _existingSymbolsMap[symbolKey].Id,
                FullName = _downloadedSymbolsMap[symbolKey].FullName,
            };

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

        public bool SafeThreshold(ExchangeSyncSetting setting, out string error)
        {
            error = null;

            int changePercent = _changedSymbols.Count / _existingSymbolsMap.Count * 100;

            if (changePercent < setting.UpdateThresholdPercent){
                return true;
            }
            else
            {
                error = $"Threshold exceeded for {nameof(SymbolDelistChangeset)}: actual:{changePercent} vs threshold: {setting.UpdateThresholdPercent}";
                return false;
            }
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
