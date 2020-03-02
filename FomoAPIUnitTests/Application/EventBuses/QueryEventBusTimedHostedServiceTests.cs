using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QueryEventBusTimedHostedServiceTests
    {
        private enum EventType
        {
            SetMaxQueryPerMinuteThreshold,
            ResetQueryExecutedCounter,
            EnqueueNextQueries,
            ExecutePendingQueriesAsync
        }

        private readonly QueryEventBusTimedHostedService _hostedService;
        private readonly Mock<ILogger<QueryEventBusTimedHostedService>> _mockLogger;
        private readonly Mock<QueryEventBus> _mockEventBus;
        private readonly Mock<IOptionsMonitor<EventBusOptions>> _mockEventBusOptionsAccessor;
        private readonly List<EventType> eventList;

        public QueryEventBusTimedHostedServiceTests()
        {
            _mockLogger = new Mock<ILogger<QueryEventBusTimedHostedService>>();
            _mockEventBus = new Mock<QueryEventBus>();
            _hostedService = new QueryEventBusTimedHostedService(_mockLogger.Object, _mockEventBus.Object, _mockEventBusOptionsAccessor.Object);
            eventList = new List<EventType>();
        }


        public void ShouldConfigureAndRunEventBusPeriodically()
        {
            var options = new EventBusOptions
            {
                MaxQueriesPerMinute = 5,
                DelayMSRunScheduleEventBus = 1,
            };

            _mockEventBusOptionsAccessor.Setup(x => x.CurrentValue).Returns(options);

            _mockEventBus.Setup(x => x.SetMaxQueryPerMinuteThreshold(options.MaxQueriesPerMinute)).Callback((int x) =>
            {
                eventList.Add(EventType.SetMaxQueryPerMinuteThreshold);
            });

            _mockEventBus.Setup(x => x.ResetQueryExecutedCounter()).Callback((int x) =>
            {
                eventList.Add(EventType.ResetQueryExecutedCounter);
            });

            _mockEventBus.Setup(x => x.EnqueueNextQueries()).Callback(() =>
            {
                eventList.Add(EventType.EnqueueNextQueries);
            });

            _mockEventBus.Setup(x => x.ExecutePendingQueriesAsync()).Callback(() =>
            {
                eventList.Add(EventType.ExecutePendingQueriesAsync);
            });
        }


       
    }
}
