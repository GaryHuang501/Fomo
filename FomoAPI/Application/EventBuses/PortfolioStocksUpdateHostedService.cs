using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.Services;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Domain.Stocks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using FomoAPI.Application.Queries;
using System.Linq;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Singleton Scheduler to update stocks that users have in their portfolio.
    /// </summary>
    /// <remarks>This is mainly to sync the leader board. This scheduler
    /// will only run outside of market hours and only if data is stale.</remarks>
    public class PortfolioStocksUpdateHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly PortfolioStocksUpdateOptions _options;
        private readonly IStockDataService _stockDataService;
        private readonly IPortfolioStocksUpdateQuery _portfolioStocksUpdateQuery;
        private readonly IMarketHours _marketHours;

        private Timer _timerExecute;

        public PortfolioStocksUpdateHostedService(
            IPortfolioStocksUpdateQuery portfolioStocksUpdateQuery,
            IStockDataService stockDataService,
            ILogger<PortfolioStocksUpdateHostedService> logger,
            IMarketHours marketHours,
            IOptionsMonitor<PortfolioStocksUpdateOptions> optionsAccesor
            )
        {
            _stockDataService = stockDataService;
            _portfolioStocksUpdateQuery = portfolioStocksUpdateQuery;
            _logger = logger;
            _marketHours = marketHours;
            _options = optionsAccesor.CurrentValue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Source} is starting", nameof(PortfolioStocksUpdateHostedService));

            _timerExecute = new(async state =>
            {
                if (_marketHours.IsMarketHours(DateTime.UtcNow))
                {
                    return;
                }

                await UpdateStocks();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_options.IntervalSeconds));

            return Task.CompletedTask;
        }

        private async Task UpdateStocks()
        {
            // Only need to update stocks with data last updated before market closed, since the price will no longer change for the day.
            // When the market closes, UTC will still be on the same day.
            
            var maxUpdateDate = _marketHours.TodayEndDateUTC().AddMinutes(StockUpdateTimeBufferRoom.Minutes);

            var symbolIdsForUpdate = await _portfolioStocksUpdateQuery.Get(_options.BatchSize, maxUpdateDate);

            _logger.LogInformation("{Source} is Updating {Updatecount} stocks", nameof(PortfolioStocksUpdateHostedService), symbolIdsForUpdate.Count());

            foreach (var symbolId in symbolIdsForUpdate)
            {
                _stockDataService.AddSubscriberToSingleQuote(symbolId);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Source} is stopping", nameof(PortfolioStocksUpdateHostedService));

            _timerExecute?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerExecute?.Dispose();
        }
    }
}
