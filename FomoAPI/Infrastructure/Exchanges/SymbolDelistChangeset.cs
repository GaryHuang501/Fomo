using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class SymbolDelistChangeset : IExchangeSyncChangeset
    {
        private List<int> _delistedSymbolIds;
        private IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public SymbolDelistChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _delistedSymbolIds = new List<int>();
            _existingSymbolsMap = existingSymbolsMap;
            _downloadedSymbolsMap = downloadedSymbolsMap;
        }

        /// <summary>
        /// Add Symbol to changeset to be delisted
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        public void AddChange(SymbolKey symbolKey)
        {
            _delistedSymbolIds.Add(_existingSymbolsMap[symbolKey].Id);
        }

        /// <summary>
        /// Check if the given symbol is delisted or not.
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        /// <returns>True if symbol is new. Otherwise False.</returns>
        public bool HasChanges(SymbolKey symbolKey)
        {
            return _existingSymbolsMap.ContainsKey(symbolKey) && !_downloadedSymbolsMap.ContainsKey(symbolKey);
        }

        public ThresholdCheck GetThresholdCheck(ExchangeSyncSetting setting)
        {
            return new ThresholdCheck(
                nameof(SymbolDelistChangeset),
                _delistedSymbolIds.Count,
                setting.DeleteThresholdPercent);
        }

        /// <summary>
        /// Delisted the symbols in the changeset.
        /// </summary>
        /// <param name="repository"><see cref="IExchangeSyncRepository"/> to make delisting update to database.</param>
        public async Task SaveChangeset(IExchangeSyncRepository repository)
        {
            if (_delistedSymbolIds.Count == 0) return;

            var delistedCount = await repository.DelistSymbols(_delistedSymbolIds);

            await repository.AddSyncHistory(nameof(SymbolDelistChangeset), delistedCount, $"{delistedCount} symbols delisted");
        }
    }
}
