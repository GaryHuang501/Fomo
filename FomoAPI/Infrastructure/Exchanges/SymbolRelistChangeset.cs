using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class SymbolRelistChangeset : IExchangeSyncChangeset
    {
        private readonly List<int> _relistedSymbolIds;
        private readonly IReadOnlyDictionary<SymbolKey, Symbol> _existingSymbolsMap;
        private readonly IReadOnlyDictionary<SymbolKey, DownloadedSymbol> _downloadedSymbolsMap;

        public SymbolRelistChangeset(IReadOnlyDictionary<SymbolKey, Symbol> existingSymbolsMap, IReadOnlyDictionary<SymbolKey, DownloadedSymbol> downloadedSymbolsMap)
        {
            _relistedSymbolIds = new List<int>();
            _existingSymbolsMap = existingSymbolsMap;
            _downloadedSymbolsMap = downloadedSymbolsMap;
        }

        /// <summary>
        /// Add Symbol to changeset to be Relisted
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        public void AddChange(SymbolKey symbolKey)
        {
            _relistedSymbolIds.Add(_existingSymbolsMap[symbolKey].Id);
        }

        /// <summary>
        /// Check if the given symbol is Relisted or not.
        /// </summary>
        /// <param name="symbolKey">The <see cref="SymbolKey"/> to identify the symbol.</param>
        /// <returns>True if symbol is new. Otherwise False.</returns>
        public bool HasChanges(SymbolKey symbolKey)
        {
            return _existingSymbolsMap.ContainsKey(symbolKey) && _downloadedSymbolsMap.ContainsKey(symbolKey)
                && _existingSymbolsMap[symbolKey].Delisted;
        }

        public ThresholdCheck GetThresholdCheck(ExchangeSyncSetting setting)
        {
            return new ThresholdCheck(
                nameof(SymbolRelistChangeset),
                _relistedSymbolIds.Count,
                setting.DeleteThresholdPercent);
        }

        /// <summary>
        /// Relisted the symbols in the changeset.
        /// </summary>
        /// <param name="repository"><see cref="IExchangeSyncRepository"/> to make Relisting update to database.</param>
        public async Task SaveChangeset(IExchangeSyncRepository repository)
        {
            if (_relistedSymbolIds.Count == 0) return;

            var RelistedCount = await repository.RelistSymbols(_relistedSymbolIds);

            await repository.AddSyncHistory(nameof(SymbolRelistChangeset), RelistedCount, $"{RelistedCount} symbols Relisted");
        }
    }
}
