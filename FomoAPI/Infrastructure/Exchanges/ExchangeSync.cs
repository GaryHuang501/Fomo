using FomoAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class ExchangeSync : IExchangeSync
    {
        private readonly IExchangeClient _exchangeClient;
        private readonly IExchangeSyncRepository _syncRepository;
        private readonly IExchangeSyncChangesetsFactory _exchangeSyncChangesetsFactory;
        private readonly ILogger<ExchangeSync> _logger;

        public ExchangeSync(IExchangeClient exchangeClient, IExchangeSyncRepository syncRepository, IExchangeSyncChangesetsFactory exchangeSyncChangesetsFactory, ILogger<ExchangeSync> logger)
        {
            _exchangeClient = exchangeClient;
            _syncRepository = syncRepository;
            _exchangeSyncChangesetsFactory = exchangeSyncChangesetsFactory;
            _logger = logger;
        }

        public async Task Sync()
        {
            try
            {
                var syncSettings = await _syncRepository.GetSyncSettings();

                if (syncSettings.DisableSync) return;

                var tickerToDownloadedSymbolMap = await _exchangeClient.GetTradedSymbols(syncSettings);
                var tickerToExistingSymbolMap = await _syncRepository.GetExistingSymbols();
                var syncChangesets = _exchangeSyncChangesetsFactory.Create(tickerToExistingSymbolMap, tickerToDownloadedSymbolMap);

                // Union the keys so new symbols from exchanges, and old symbols on int database are checked for changes.
                var intersectingSymbolsKeys = tickerToDownloadedSymbolMap.Keys.Union(tickerToExistingSymbolMap.Keys);

                foreach (var symbolKey in intersectingSymbolsKeys)
                {
                    foreach (var changeset in syncChangesets)
                    {
                        if (changeset.HasChanges(symbolKey))
                        {
                            changeset.AddChange(symbolKey);
                        }
                    }
                }

                foreach (var changeset in syncChangesets)
                {
                    if (!syncSettings.DisableThresholds)
                    {
                        ThresholdCheck thresholdCheck = changeset.GetThresholdCheck(syncSettings);
                        bool success = thresholdCheck.CheckThreshold(tickerToExistingSymbolMap.Count, out string error);

                        if (!success)
                        {
                            _logger.LogError(error);
                            throw new ExchangeSyncException(error);
                        }
                    }

                    await changeset.SaveChangeset(_syncRepository);
                }

            }
            catch (Exception ex)
            {
                await _syncRepository.AddSyncHistory("Exception", 0, ex.Message, ex.ToString());
                _logger.LogError(ex.Message, ex);
                throw new ExchangeSyncException("Error syncing with Exchange", ex);
            }
        }
    }
}
