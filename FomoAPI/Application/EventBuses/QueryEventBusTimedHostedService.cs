using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses.QueryExecutorContexts;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Singleton Scheduler that will trigger the event bus to run the queries every minute
    /// </summary>
    public class QueryEventBusTimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IQueryEventBus _queryEventBus;
        private readonly EventBusOptions _eventBusOptions;
        private Timer _timerSetEventBusState;
        private Timer _timerExecuteEventBus;

        public QueryEventBusTimedHostedService(ILogger<QueryEventBusTimedHostedService> logger, IQueryEventBus queryEventBus, IOptionsMonitor<EventBusOptions> eventBusOptionsAccessor)
        {
            _logger = logger;
            _queryEventBus = queryEventBus;
            _eventBusOptions = eventBusOptionsAccessor.CurrentValue;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} is starting");
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} will reset state and run any pending queries every {_eventBusOptions.DelayMSRunScheduleEventBus} seconds");


            _timerExecuteEventBus = new Timer((state) =>
            {
                SetEventBusState();
                EnqueuePendingQueries();
                ExecuteEventBus();
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(_eventBusOptions.DelayMSRunScheduleEventBus));

            return Task.CompletedTask;
        }

        private void ValidateOptions()
        {
            if (_eventBusOptions.DelayMSRunScheduleEventBus <= 0)
            {
                var error = $"{nameof(_eventBusOptions.DelayMSRunScheduleEventBus)} cannot be less than or equal to 0";
                _logger.LogError(error);
                throw new ArgumentException(error);
            }

            if (_eventBusOptions.MaxQueriesPerMinute <= 0)
            {
                var error = $"{nameof(_eventBusOptions.MaxQueriesPerMinute)} cannot be less than or equal to 0";
                _logger.LogError(error);
                throw new ArgumentException(error);
            }
        }
        private void EnqueuePendingQueries()
        {
            _logger.LogInformation($"Enqueuing next set of queries");
            _queryEventBus.EnqueueNextQueries();
        }

        private async void SetEventBusState()
        {
            _logger.LogInformation($"Resetting Event Bus state");
            _logger.LogInformation($"Max queries to run per minute is {_eventBusOptions.MaxQueriesPerMinute}");

            try
            {
                _queryEventBus.SetMaxQueryPerMinuteThreshold(_eventBusOptions.MaxQueriesPerMinute);
                _queryEventBus.ResetQueryExecutedCounter();
                await _queryEventBus.ExecutePendingQueriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error refreshing event bus state state" );
            }
        }

        private async void ExecuteEventBus()
        {
            _logger.LogInformation($"Scheduling event bus queries");

            try
            {
                await _queryEventBus.ExecutePendingQueriesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing queries");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(QueryEventBusTimedHostedService)} is stopping");

            _timerSetEventBusState?.Change(Timeout.Infinite, 0);
            _timerExecuteEventBus?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerSetEventBusState.Dispose();
            _timerExecuteEventBus?.Dispose();
        }
    }
}
