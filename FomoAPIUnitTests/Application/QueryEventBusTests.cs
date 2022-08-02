using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Application
{
    public class QueryEventBusTests : IAsyncLifetime
    {
        private readonly QueryEventBus _queryEventBus;

        private readonly QuerySubscriptions _querySubscriptions;

        private readonly QueryQueue _queryQueue;

        private readonly Mock<IQueryContextFactory> _mockContextFactory;

        private readonly Mock<IMarketHours> _mockMarketHours;

        private record TestQuery : StockQuery
        {
            public Mock<IQueryContext> MockQueryContext { get; private set; }

            public TestQuery(int symbolId)
                : base (symbolId, QueryFunctionType.SingleQuote)
            {
                MockQueryContext = new Mock<IQueryContext>();

                // Setting it to null will tell QuerySubscription that this query is not in the cache
                // So it will say this query needs to be updated/fetched.
                MockQueryContext.Setup(c => c.GetCachedQueryResult(SymbolId)).Returns(Task.FromResult<StockQueryResult>(null));
            }

            public override IQueryContext CreateContext(IQueryContextFactory contextFactory)
            {             
                return MockQueryContext.Object;
            }
        }

        public QueryEventBusTests()
        {
            _querySubscriptions = new QuerySubscriptions();

            _queryQueue = new QueryQueue();

            // Will allow same queries to be queued again after a  millisecond
            _queryQueue.SetIntervalKey(() => DateTime.UtcNow.Millisecond);

            _mockContextFactory = new Mock<IQueryContextFactory>();

            _mockMarketHours = new Mock<IMarketHours>();

            // Make it some time way in the future, so data is considered stale.
            _mockMarketHours.Setup(h => h.TodayEndDateUTC()).Returns(new DateTime(2100, 1, 1));

            var priorityRule = new QuerySubscriptionCountRule(
                    contextFactory: _mockContextFactory.Object,
                    _querySubscriptions,
                    _mockMarketHours.Object,
                    (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object
                );

            _queryEventBus = new QueryEventBus(
                    queryQueue: _queryQueue,
                    queryContextFactory: _mockContextFactory.Object,
                    queuePriorityRule: priorityRule,
                    logger: (new Mock<ILogger<QueryEventBus>>()).Object
                );

        }

        public async Task InitializeAsync()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(3);
            await _queryEventBus.Reset();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Should_ExecuteQueries()
        {
            var queries = CreateQueries(3);

            queries.ForEach(q => _querySubscriptions.AddSubscriber(q));

            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once));

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_NotifyUpdatedQueries()
        {
            var queries = CreateQueries(3);

            queries.ForEach(q => _querySubscriptions.AddSubscriber(q));

            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.NotifyChangesClients(), Times.Once));
        }

        [Fact]
        public async Task Should_NotExceedMaxQueriesForInterval()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var queries = CreateQueries(3);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);

            _querySubscriptions.AddSubscriber(queries[1]);
            _querySubscriptions.AddSubscriber(queries[1]);

            _querySubscriptions.AddSubscriber(queries[2]);

            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            //Executed queries get subscriber count cleared.
            Assert.Equal(1, _querySubscriptions.Count);
            Assert.Equal(1, _querySubscriptions.GetSubscriberCount(queries[2]));
        }

        [Fact]
        public async Task Should_NotExceedMaxQueriesForInterval_MultipleExecution()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var queries = CreateQueries(3);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);

            _querySubscriptions.AddSubscriber(queries[1]);
            _querySubscriptions.AddSubscriber(queries[1]);

            _querySubscriptions.AddSubscriber(queries[2]);

            await _queryEventBus.ExecutePendingQueries(); 
            await _queryEventBus.ExecutePendingQueries();
            await _queryEventBus.ExecutePendingQueries();
            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
        }

        [Fact]
        public async Task Should_ExecuteQuerieNextInterval_WhenFirstIntervalMaxQueryNotReached()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(3);
            await _queryEventBus.Reset();

            var queries = CreateQueries(4);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[1]);

            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            queries[3].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            // add twice to make sure it is prioritized over 4 for easier testing.
            _querySubscriptions.AddSubscriber(queries[2]);
            _querySubscriptions.AddSubscriber(queries[2]);

            _querySubscriptions.AddSubscriber(queries[3]);

            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[3].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once));

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_ExecuteQuerieNextInterval_WhenFirstIntervalMaxQueryReached()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var queries = CreateQueries(4);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[1]);

            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            queries[3].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            _querySubscriptions.AddSubscriber(queries[2]);
            _querySubscriptions.AddSubscriber(queries[3]);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once));

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_ExecuteQueriesByPriority_SubscriberCount_MultipleIntervals()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(1);
            await _queryEventBus.Reset();

            var queries = CreateQueries(3);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);

            _querySubscriptions.AddSubscriber(queries[1]);
            _querySubscriptions.AddSubscriber(queries[1]);

            _querySubscriptions.AddSubscriber(queries[2]);

            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
        }

        [Fact]
        public async Task Should_HandleConcurrency_ExecuteQueryOnceAndNotExceedThreshold()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var queries = CreateQueries(3);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[0]);

            _querySubscriptions.AddSubscriber(queries[1]);
            _querySubscriptions.AddSubscriber(queries[1]);

            _querySubscriptions.AddSubscriber(queries[2]);

            var executionTasks = new List<Task>();

            for(int  i = 0; i < 10; i++)
            {
                executionTasks.Add(Task.Run( async () => await _queryEventBus.ExecutePendingQueries()));
            }

            await Task.WhenAll(executionTasks);

            queries[0].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[1].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            queries[2].MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            Assert.Equal(1, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_HandleConcurrency_ResetShouldNotAffectRunningQueries()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(50);
            await _queryEventBus.Reset();

            var queries = CreateQueries(6);

            queries.ForEach(q => _querySubscriptions.AddSubscriber(q));

            var executionTasks = new List<Task>();

            for (int i = 0; i < 30; i++)
            {
                if (i % 3 == 0)
                {
                    executionTasks.Add(Task.Run(async () => await _queryEventBus.Reset()));
                }
                else
                {
                    executionTasks.Add(Task.Run( async() => await _queryEventBus.ExecutePendingQueries()));
                }
            }

            await Task.WhenAll(executionTasks);

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once));

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_BeAbleToExecuteNextQueryBatch_WhenErrorEnqueing_MultipleBatches()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(100);
            await _queryEventBus.Reset();

            var queries = CreateQueries(6);

            _querySubscriptions.AddSubscriber(queries[0]);
            _querySubscriptions.AddSubscriber(queries[1]);

            queries[1].MockQueryContext.Setup(c => c.GetCachedQueryResult(queries[1].SymbolId)).Throws(new Exception("Force exception"));

            try
            {
                await _queryEventBus.ExecutePendingQueries();
            }
            catch(Exception)
            {
            }

            queries[1].MockQueryContext.Setup(c => c.GetCachedQueryResult(queries[1].SymbolId)).Returns(Task.FromResult<StockQueryResult>(null));
            await _queryEventBus.ExecutePendingQueries();

            _querySubscriptions.AddSubscriber(queries[2]);
            _querySubscriptions.AddSubscriber(queries[3]);

            await _queryEventBus.ExecutePendingQueries();

            _querySubscriptions.AddSubscriber(queries[4]);
            _querySubscriptions.AddSubscriber(queries[5]);

            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once));
        }

        [Fact]
        public async Task Should_ContinueToExecuteQueries_WhenErrorExecuting_MultipleQueries()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(5);
            await _queryEventBus.Reset();

            var queries = CreateQueries(5);

            queries.ForEach(q => _querySubscriptions.AddSubscriber(q));

            queries[1].MockQueryContext.Setup(c => c.SaveQueryResultToStore()).Throws(new Exception("Force exception"));

            await _queryEventBus.ExecutePendingQueries();

            queries.ForEach(q => q.MockQueryContext.Verify( c => c.SaveQueryResultToStore(), Times.Once));
        }

        private List<TestQuery> CreateQueries(int num)
        {
            var queries = new List<TestQuery>();

            for(var i = 0; i < num; i++)
            {
                queries.Add(new TestQuery(num + 1));
            }

            return queries;
        }

        private async Task WaitNextInterval()
        {
            await Task.Delay(10);
        }
    }
}
