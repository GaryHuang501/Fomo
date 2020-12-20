using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Singleton Scheduler that will trigger the event bus to run the queries every specified interval
    /// </summary>
    public class QueryEventBusTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IQueryEventBus _queryEventBus;
        private readonly EventBusOptions _eventBusOptions;
        private Timer _timerExecuteEventBus; 
        private Timer _timerRefreshEventBus;

        public QueryEventBusTimedHostedService(ILogger<QueryEventBusTimedHostedService> logger, IQueryEventBus queryEventBus, IOptionsMonitor<EventBusOptions> eventBusOptionsAccessor)
        {
            _logger = logger;
            _queryEventBus = queryEventBus;
            _eventBusOptions = eventBusOptionsAccessor.CurrentValue;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} is starting");
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} will reset state and run any pending queries every {_eventBusOptions.RefreshIntervalMS} milliseconds");
            await RefreshEventBusState();

            ValidateOptions();

            _timerExecuteEventBus = new Timer(async state =>
            {
                await ExecuteEventBus();
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_eventBusOptions.PollingIntervalMS));


            _timerRefreshEventBus = new Timer( async state =>
            {
                await RefreshEventBusState();
            }, null, TimeSpan.FromMilliseconds(_eventBusOptions.RefreshIntervalMS), TimeSpan.FromMilliseconds(_eventBusOptions.RefreshIntervalMS));

            return;
        }

        private void ValidateOptions()
        {
            if(_eventBusOptions.PollingIntervalMS <= 0)
            {
                throw new ArgumentException(nameof(_eventBusOptions.PollingIntervalMS), "cannot be less than 1");
            }

            if (_eventBusOptions.MaxQueriesPerInterval <= 0)
            {
                throw new ArgumentException(nameof(_eventBusOptions.MaxQueriesPerInterval), "cannot be less than 1");
            }

            if (_eventBusOptions.RefreshIntervalMS <= 0)
            {
                throw new ArgumentException(nameof(_eventBusOptions.RefreshIntervalMS), "cannot be less than 1");
            }
        }

        private async Task RefreshEventBusState()
        {
            _logger.LogInformation("Resetting Event Bus state");
            _logger.LogInformation($"Max queries to run per interval is {_eventBusOptions.MaxQueriesPerInterval}");

            try
            {
                _queryEventBus.SetMaxQueryPerIntervalThreshold(_eventBusOptions.MaxQueriesPerInterval);
                await _queryEventBus.Reset();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing event bus state state" );
            }
        }

        private async Task ExecuteEventBus()
        {
            _logger.LogInformation("Scheduling event bus queries");

            try
            {
                await _queryEventBus.ExecutePendingQueries();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing queries");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} is stopping");

            _timerExecuteEventBus?.Change(Timeout.Infinite, 0);
            _timerRefreshEventBus?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerExecuteEventBus?.Dispose();
            _timerRefreshEventBus?.Dispose();

        }
    }
}
