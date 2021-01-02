using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
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
        private class TestQuery : StockQuery
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

            var priorityRule = new QuerySubscriptionCountRule(
                    contextFactory: _mockContextFactory.Object,
                    _querySubscriptions,
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
            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2); 
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query3);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);


            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_NotifyUpdatedQueries()
        {
            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query3);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.NotifyChangesClients(), Times.Once);
            query2.MockQueryContext.Verify(c => c.NotifyChangesClients(), Times.Once);
            query3.MockQueryContext.Verify(c => c.NotifyChangesClients(), Times.Once);
        }

        [Fact]
        public async Task Should_NotExceedMaxQueriesForInterval()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);

            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query2);

            _querySubscriptions.AddSubscriber(query3);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            //Executed queries get subscriber count cleared.
            Assert.Equal(1, _querySubscriptions.Count);
            Assert.Equal(1, _querySubscriptions.GetSubscriberCount(query3));
        }

        [Fact]
        public async Task Should_NotExceedMaxQueriesForInterval_MultipleExecution()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);

            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query2);

            _querySubscriptions.AddSubscriber(query3);

            await _queryEventBus.ExecutePendingQueries(); 
            await _queryEventBus.ExecutePendingQueries();
            await _queryEventBus.ExecutePendingQueries();
            await _queryEventBus.ExecutePendingQueries();


            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
        }

        [Fact]
        public async Task Should_ExecuteQuerieNextInterval_WhenFirstIntervalMaxQueryNotReached()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(3);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3); 
            var query4 = new TestQuery(4);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never); 
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            // add twice to make sure it is prioritized over 4 for easier testing.
            _querySubscriptions.AddSubscriber(query3);
            _querySubscriptions.AddSubscriber(query3);

            _querySubscriptions.AddSubscriber(query4);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_ExecuteQuerieNextInterval_WhenFirstIntervalMaxQueryReached()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);
            var query4 = new TestQuery(4);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            _querySubscriptions.AddSubscriber(query3);
            _querySubscriptions.AddSubscriber(query4);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);

            Assert.Equal(0, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_ExecuteQueriesByPriority_SubscriberCount_MultipleIntervals()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(1);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);

            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query2);

            _querySubscriptions.AddSubscriber(query3);

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);

            await WaitNextInterval();
            await _queryEventBus.Reset();
            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
        }

        [Fact]
        public async Task Should_HandleConcurrency_ExecuteQueryOnceAndNotExceedThreshold()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(2);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query1);

            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query2);

            _querySubscriptions.AddSubscriber(query3);

            var executionTasks = new List<Task>();

            for(int  i = 0; i < 10; i++)
            {
                executionTasks.Add(Task.Run( async () => await _queryEventBus.ExecutePendingQueries()));
            }

            await Task.WhenAll(executionTasks);

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Never);
            Assert.Equal(1, _querySubscriptions.Count);
        }

        [Fact]
        public async Task Should_HandleConcurrency_ResetShouldNotAffectRunningQueries()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(50);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);
            var query4 = new TestQuery(4);
            var query5 = new TestQuery(5);
            var query6 = new TestQuery(6);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query3);
            _querySubscriptions.AddSubscriber(query4);
            _querySubscriptions.AddSubscriber(query5);
            _querySubscriptions.AddSubscriber(query6);

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

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query5.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query6.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);

            Assert.Equal(0, _querySubscriptions.Count);
        }


        [Fact]
        public async Task Should_BeAbleToExecuteNextQueryBatch_WhenErrorEnqueing_MultipleBatches()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(100);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);
            var query4 = new TestQuery(4);
            var query5 = new TestQuery(5); 
            var query6 = new TestQuery(6);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);
            query2.MockQueryContext.Setup(c => c.GetCachedQueryResult(query2.SymbolId)).Throws(new Exception("Force exception"));

            try
            {
                await _queryEventBus.ExecutePendingQueries();
            }
            catch(Exception)
            {
            }

            query2.MockQueryContext.Setup(c => c.GetCachedQueryResult(query2.SymbolId)).Returns(Task.FromResult<StockQueryResult>(null));
            await _queryEventBus.ExecutePendingQueries();

            _querySubscriptions.AddSubscriber(query3);
            _querySubscriptions.AddSubscriber(query4);

            await _queryEventBus.ExecutePendingQueries();

            _querySubscriptions.AddSubscriber(query5);
            _querySubscriptions.AddSubscriber(query6);


            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query5.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query6.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
        }

        [Fact]
        public async Task Should_ContinueToExecuteQueries_WhenErrorExecuting_MultipleQueries()
        {
            await _queryEventBus.SetMaxQueryPerIntervalThreshold(5);
            await _queryEventBus.Reset();

            var query1 = new TestQuery(1);
            var query2 = new TestQuery(2);
            var query3 = new TestQuery(3);
            var query4 = new TestQuery(4);
            var query5 = new TestQuery(5);

            _querySubscriptions.AddSubscriber(query1);
            _querySubscriptions.AddSubscriber(query2);
            _querySubscriptions.AddSubscriber(query3);
            _querySubscriptions.AddSubscriber(query4);
            _querySubscriptions.AddSubscriber(query5);

            query2.MockQueryContext.Setup(c => c.SaveQueryResultToStore()).Throws(new Exception("Force exception"));

            await _queryEventBus.ExecutePendingQueries();

            query1.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query2.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query3.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query4.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
            query5.MockQueryContext.Verify(c => c.SaveQueryResultToStore(), Times.Once);
        }

        private async Task WaitNextInterval()
        {
            await Task.Delay(10);
        }
    }
}
