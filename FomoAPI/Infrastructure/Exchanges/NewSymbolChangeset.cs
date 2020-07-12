using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Symbol changeset to sync in new symbols from exchange to database.
    /// </summary>
    public class NewSymbolChangeset : IExchangeSyncChangeset
    {
        private List<Symbol> _newSymbols;
        private IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public NewSymbolChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _newSymbols = new List<Symbol>();
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
                ExchangeId = symbolKey.ExchangeId,
                Delisted = false,
                FullName = _downloadedSymbolsMap[symbolKey].FullName,
                Ticker = symbolKey.Ticker
            };

            _newSymbols.Add(symbolForDb);
        }

        /// <summary>
        /// Check if the given symbol is new or not.
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        /// <returns>True if symbol is new. Otherwise False.</returns>
        public bool HasChanges(SymbolKey symbolKey)
        {
            return !_existingSymbolsMap.ContainsKey(symbolKey) && _downloadedSymbolsMap.ContainsKey(symbolKey);
        }

        public bool SafeThreshold(ExchangeSyncSetting setting, out string error)
        {
            error = null;
            int changePercent = _newSymbols.Count / _existingSymbolsMap.Count * 100;

            if (changePercent < setting.InsertThresholdPercent)
            {
                return true;
            }
            else
            {
                error = $"Threshold exceeded for {nameof(NewSymbolChangeset)}: actual:{changePercent} vs threshold: {setting.InsertThresholdPercent}";
                return false;
            }
        }

        /// <summary>
        /// Saves all new symbols to database.
        /// </summary>
        /// <param name="repository"><see cref="IExchangeSyncRepository"/> to save to database.</param>
        public async Task SaveChangeset(IExchangeSyncRepository repository)
        {
            if (_newSymbols.Count == 0) return;

            var insertedCount = await repository.AddSymbols(_newSymbols);

            await repository.AddSyncHistory(nameof(NewSymbolChangeset), insertedCount, $"{insertedCount} symbols inserted");
        }
    }
}
