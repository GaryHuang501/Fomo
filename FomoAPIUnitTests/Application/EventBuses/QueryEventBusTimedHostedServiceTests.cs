﻿using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QueryEventBusTimedHostedServiceTests
    {
        private enum EventType
        {
            SetMaxQueryPerInterval,
            ResetQueryExecutedCounter,
            EnqueueNextQueries,
            ExecutePendingQueriesAsync
        }

        private readonly Mock<ILogger<QueryEventBusTimedHostedService>> _mockLogger;
        private readonly Mock<IQueryEventBus> _mockEventBus;
        private readonly Mock<IOptionsMonitor<EventBusOptions>> _mockEventBusOptionsAccessor;
        private readonly List<EventType> _eventList;
        private int _mockEventBusSetInterval;

        public QueryEventBusTimedHostedServiceTests()
        {
            _mockLogger = new Mock<ILogger<QueryEventBusTimedHostedService>>();
            _mockEventBus = new Mock<IQueryEventBus>();
            _mockEventBusOptionsAccessor = new Mock<IOptionsMonitor<EventBusOptions>>();
            _eventList = new List<EventType>();
            _mockEventBusSetInterval = 0;
        }

        private void SetupEventBusStub(EventBusOptions options)
        {
            _mockEventBusOptionsAccessor.Setup(x => x.CurrentValue).Returns(options);

            _mockEventBus.Setup(x => x.SetMaxQueryPerIntervalThreshold(options.MaxQueriesPerInterval)).Callback((int x) =>
            {
                _eventList.Add(EventType.SetMaxQueryPerInterval);
                _mockEventBusSetInterval = x;
            });

            _mockEventBus.Setup(x => x.ResetQueryExecutedCounter()).Callback(() =>
            {
                _eventList.Add(EventType.ResetQueryExecutedCounter);
            });

            _mockEventBus.Setup(x => x.EnqueueNextQueries()).Callback(() =>
            {
                _eventList.Add(EventType.EnqueueNextQueries);
            });

            _mockEventBus.Setup(x => x.ExecutePendingQueriesAsync()).Callback(() =>
            {
                _eventList.Add(EventType.ExecutePendingQueriesAsync);
            });
        }

        [Fact]
        public async Task ShouldConfigureAndRunEventBus()
        {
            // Make the delay very large so we only test one interval run
            var options = new EventBusOptions
            {
                MaxQueriesPerInterval = 5,
                IntervalLengthMS = 10000,
            };

            SetupEventBusStub(options);

            using (var hostedService = new QueryEventBusTimedHostedService(_mockLogger.Object, _mockEventBus.Object, _mockEventBusOptionsAccessor.Object))
            {
                await hostedService.StartAsync(new CancellationToken());
                await WaitForEvent(EventType.ExecutePendingQueriesAsync);

                Assert.Equal(4, _eventList.Count);

                Assert.Collection(_eventList,
                    event1 => Assert.Equal(EventType.SetMaxQueryPerInterval, event1),
                    event2 => Assert.Equal(EventType.ResetQueryExecutedCounter, event2),
                    event3 => Assert.Equal(EventType.EnqueueNextQueries, event3),
                    event4 => Assert.Equal(EventType.ExecutePendingQueriesAsync, event4)
                 );

                Assert.Equal(options.MaxQueriesPerInterval, _mockEventBusSetInterval);
            }
        }

        [Fact]
        public async Task ShouldConfigureAndRunEventBus_MultipleIntervals()
        {
            int waitMs = 1000;

            var options = new EventBusOptions
            {
                MaxQueriesPerInterval = 5,
                IntervalLengthMS = 100, // use a bigger interval for less noise
            };

            SetupEventBusStub(options);

            using (var hostedService = new QueryEventBusTimedHostedService(_mockLogger.Object, _mockEventBus.Object, _mockEventBusOptionsAccessor.Object))
            {
                await hostedService.StartAsync(new CancellationToken());
                await Task.Delay(waitMs);

                var approximateNumberOfExpectedCalls = (waitMs / options.IntervalLengthMS);
                var actualNumberOfCalls = _eventList.Count(e => e == EventType.ResetQueryExecutedCounter);
                Assert.True(Math.Abs(actualNumberOfCalls - approximateNumberOfExpectedCalls)  < 2 );
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ShouldThrowExceptionWhenMaxQueriesSettingEqualOrLessThanZero(int settingValue)
        {
            var options = new EventBusOptions
            {
                MaxQueriesPerInterval = settingValue,
                IntervalLengthMS = 100, // use a bigger interval for less noise
            };

            SetupEventBusStub(options);

            using (var hostedService = new QueryEventBusTimedHostedService(_mockLogger.Object, _mockEventBus.Object, _mockEventBusOptionsAccessor.Object))
            {
                await Assert.ThrowsAsync<ArgumentException>(() => hostedService.StartAsync(new CancellationToken()));

            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task ShouldThrowExceptionWhenIntervalDelaySettingEqualOrLessThanZero(int settingValue)
        {
            var options = new EventBusOptions
            {
                MaxQueriesPerInterval = 100,
                IntervalLengthMS = settingValue, // use a bigger interval for less noise
            };

            SetupEventBusStub(options);

            using (var hostedService = new QueryEventBusTimedHostedService(_mockLogger.Object, _mockEventBus.Object, _mockEventBusOptionsAccessor.Object))
            {
                await Assert.ThrowsAsync<ArgumentException>(() => hostedService.StartAsync(new CancellationToken()));
            }
        }

        private async Task WaitForEvent(EventType eventType, int timeOutMs = 5000)
        {
            var startTime = DateTime.Now;
            while (!_eventList.Contains(eventType) && startTime > DateTime.Now.AddMilliseconds(-timeOutMs))
            {
                await Task.Delay(100);
            }
        }
    }
}
