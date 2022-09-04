﻿using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Infrastructure.Exchanges;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FomoAPI.Application.Exchanges
{
    /// <summary>
    /// Singleton Scheduler that will sync the database stock information with the source client.
    /// </summary>
    public class ExchangeSyncHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IExchangeSync _exchangeSync;
        private readonly ExchangeSyncOptions _exchangeSyncOptions;
        private Timer _timer;

        const int MinimumIntervalMinutes = 5;

        public ExchangeSyncHostedService(ILogger<ExchangeSyncHostedService> logger,
                                         IExchangeSync exchangeSync,
                                         IOptionsMonitor<ExchangeSyncOptions> exchangeSyncOptionsAccessor)
        {
            _logger = logger;
            _exchangeSync = exchangeSync;
            _exchangeSyncOptions = exchangeSyncOptionsAccessor.CurrentValue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Source} is Starting", nameof(ExchangeSyncHostedService));

            int minutesInterval = Math.Max(_exchangeSyncOptions.SyncIntervalMinutes, MinimumIntervalMinutes);

            TimeSpan startDelay = TimeSpan.FromMinutes(minutesInterval);

            if (_exchangeSyncOptions.SyncOnStart)
            {
                startDelay = TimeSpan.Zero;
            }

            _timer = new Timer(async (state) =>
            {
                try
                {
                    await _exchangeSync.Sync();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(minutesInterval));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("{Source} is stopping", nameof(ExchangeSyncHostedService));
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
