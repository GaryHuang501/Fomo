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
        private readonly List<InsertSymbolAction> _newSymbols;
        private readonly IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private readonly IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public NewSymbolChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _newSymbols = new List<InsertSymbolAction>();
            _existingSymbolsMap = existingSymbolsMap;
            _downloadedSymbolsMap = downloadedSymbolsMap;
        }

        /// <summary>
        /// Add new Symbol to changeset to be inserted into database
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        public void AddChange(SymbolKey symbolKey)
        {
            var symbolForDb = new InsertSymbolAction(
                symbolKey.Ticker, 
                symbolKey.ExchangeId, 
                _downloadedSymbolsMap[symbolKey].FullName, 
                false);

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

        public ThresholdCheck GetThresholdCheck(ExchangeSyncSetting setting)
        {
            return new ThresholdCheck(
                nameof(NewSymbolChangeset),
                _newSymbols.Count,
                setting.InsertThresholdPercent);
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
