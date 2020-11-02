using FomoAPI.Application.Stores;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryExecutorContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Options;
using FomoAPI.Application.ConfigurationOptions;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QueryEventBusScenarioTests
    {
        private readonly QueryEventBus _queryEventBus;
        private readonly QueryPrioritySet _queryQueue;
        private readonly QuerySubscriptions _querySubscriptions;
        private readonly IQueryResultStore _store;
        private readonly Mock<IStockClient> _mockAlphaVantageClient;
        private int _maxQueryPerIntervalThreshold = 5;

        public QueryEventBusScenarioTests()
        {
            var mockLogger = new Mock<ILogger<QueryEventBus>>();

            _queryQueue = new QueryPrioritySet();
            _querySubscriptions = new QuerySubscriptions();
            _mockAlphaVantageClient = new Mock<IStockClient>();

            var mockCacheOptions = new Mock<IOptionsMonitor<CacheOptions>>();
            mockCacheOptions.Setup(x => x.Get(SingleQuoteCache.CacheName)).Returns(new CacheOptions
            {
                CacheItemSize = 10,
                CacheExpiryTimeMinutes = 1,
                CacheSize = 10
            });

            _store = new QueryResultStore(new SingleQuoteCache(mockCacheOptions.Object));
           
            var alphaVantageContext = new AlphaVantageSingleQuoteQueryExecutorContext(_mockAlphaVantageClient.Object, _store);
            var contextRegistry = new QueryExecutorContextRegistry(alphaVantageContext);
            var priorityRule = new QuerySortBySubscriptionCountRule(_store);

            _queryEventBus = new QueryEventBus(_queryQueue, contextRegistry, priorityRule, mockLogger.Object, _querySubscriptions);
        }

        [Fact]
        public async Task ShouldDoNothingWhenZeroQueriesInQueue()
        {
            await TriggerEventQueueBus();
        }

        [Fact]
        public void ShouldSaveQueryResultsToStoreWhenOneQuery()
        {

        }


        [Fact]
        public void ShouldSaveQueryResultsToStoreWhenManyQueries()
        {

        }

        [Fact]
        public void ShouldLimitQueriesExecutedWhenMaxIsExceeded()
        {
        }

        [Fact]
        public void ShouldLimitQueriesExecutedWhenMaxIsExceededDuringMultipleCalls()
        {
        }

        [Fact]
        public void ShouldRunTriggersOnQueryResults()
        {

        }

        [Fact]
        public void ShouldRemoveQueryFromQueueWhenExecutionHasException()
        {

        }

        private async Task TriggerEventQueueBus()
        {
            _queryEventBus.SetMaxQueryPerIntervalThreshold(_maxQueryPerIntervalThreshold);
            _queryEventBus.ResetQueryExecutedCounter();
            _queryEventBus.EnqueueNextQueries();
            await _queryEventBus.ExecutePendingQueriesAsync();
        }

    }
}
