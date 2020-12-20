using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QueryQueueTests
    {
        private readonly ITestOutputHelper _output;

        public QueryQueueTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void IsEmpty_ShouldReturnTrue_WhenNoQueriesAdded()
        {
            var queryQueue = new QueryQueue();

            Assert.True(queryQueue.IsEmpty());
        }

        [Fact]
        public void IsEmpty_ShouldReturnFalse_WhenQueryAdded()
        {
            var queryQueue = new QueryQueue();
            var query = new TestQuery(1, QueryFunctionType.SingleQuote);

            queryQueue.Enqueue(query);

            Assert.False(queryQueue.IsEmpty());
        }

        [Fact]
        public void Count_ShouldReturnSetSize()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);

            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);

            Assert.Equal(3, queryQueue.Count);
        }

        [Fact]
        public void Dequeue_ShouldBeFIFO_WhenMultipleQueriesAdded_CoupleItems()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);

            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);

            var queries = queryQueue.Dequeue(1);
            Assert.Equal(query1, queries.Single());
        }

        [Fact]
        public void Dequeue_ShouldBeFIFO_WhenMultipleQueriesAdded_ManyItems()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);
            var query4 = new TestQuery(4, QueryFunctionType.Monthly);

            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);
            queryQueue.Enqueue(query4);

            var queries = queryQueue.Dequeue(3).ToList();
            Assert.Equal(query1, queries[0]);
            Assert.Equal(query2, queries[1]);
            Assert.Equal(query3, queries[2]);
            Assert.Equal(1, queryQueue.Count);
        }

        [Fact]
        public void Dequeue_ShouldReturnAllItems_WhenDequeueCountExceedsQueueCount()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);

            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);

            var queries = queryQueue.Dequeue(100).ToList();
            Assert.Equal(3, queries.Count);
            Assert.True(queryQueue.IsEmpty());
        }

        [Fact]
        public void ClearAll_ShouldClearEverything()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);

            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);

            queryQueue.ClearAll();

            Assert.True(queryQueue.IsEmpty());

            // can add query again which means duplication check is gone.
            Assert.True(queryQueue.Enqueue(query1));
        }

        [Fact]
        public void Enqueue_ShouldReturnTrue_WhenQueryIsAddedSuccessfully()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);

            bool isSuccessful = queryQueue.Enqueue(query1);

            Assert.True(isSuccessful);
        }

        [Fact]
        public void Enqueue_ShouldReturnFalse_AndNotAddQuery_WhenQueryIsDuplicate()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);

            queryQueue.Enqueue(query1);
            bool isSecondAddSuccessful = queryQueue.Enqueue(query1);

            Assert.False(isSecondAddSuccessful);
            Assert.Equal(1, queryQueue.Count);
        }

        [Fact]
        public void Enqueue_ShouldReturnFalse_WhenQueryIsAddedAgainAfterItWasDequeued()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);

            queryQueue.Enqueue(query1);
            queryQueue.Dequeue(1);
            bool isAddSuccessful = queryQueue.Enqueue(query1);

            Assert.False(isAddSuccessful);
        }

        [Fact]
        public void Dequeue_ShouldBeThreadSafe_FIFO()
        {
            for(var i = 0; i < 10; i++)
            {
                var queryQueue = new QueryQueue();
                var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
                var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
                var query3 = new TestQuery(3, QueryFunctionType.Weekly);
                var queryList = new List<TestQuery>() { query1, query2, query3 };
                var dequeueList = new ConcurrentBag<TestQuery>();

                queryQueue.Enqueue(query1);
                queryQueue.Enqueue(query2);
                queryQueue.Enqueue(query3);

                queryList.AsParallel().ForAll(q =>
                {
                    var dequeued = (TestQuery)queryQueue.Dequeue(1).Single();
                    dequeueList.Add(dequeued);
                });

                Assert.True(queryQueue.IsEmpty());
                Assert.Equal(3, dequeueList.Count);

                Assert.Contains(query1, dequeueList);
                Assert.Contains(query2, dequeueList);
                Assert.Contains(query3, dequeueList);
            }
        }

        [Fact]
        public void GetCurrentIntervalQueriesRanCount_ShouldReturnExecutedQueryCount_WhenAllExecuting()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);
            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);
            queryQueue.Dequeue(3);

            Assert.Equal(3, queryQueue.GetCurrentIntervalQueriesRanCount());
        }

        [Fact]
        public void GetCurrentIntervalQueriesRanCount_ShouldReturnExecutedQueryCount_WhenAllExecuted()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);
            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);
            queryQueue.MarkAsExecuted(query1);
            queryQueue.MarkAsExecuted(query2);
            queryQueue.MarkAsExecuted(query3);

            Assert.Equal(3, queryQueue.GetCurrentIntervalQueriesRanCount());
        }

        [Fact]
        public void GetCurrentIntervalQueriesRanCount_ShouldReturnExecutedAndExecutingQueryCount()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);
            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);

            queryQueue.Dequeue(3);
            queryQueue.MarkAsExecuted(query1);
            queryQueue.MarkAsExecuted(query2);

            Assert.Equal(3, queryQueue.GetCurrentIntervalQueriesRanCount());
        }


        [Fact]
        public void GetCurrentIntervalQueriesRanCount_ShouldReturnZero_WhenAllPending()
        {
            var queryQueue = new QueryQueue();
            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.Weekly);
            queryQueue.Enqueue(query1);
            queryQueue.Enqueue(query2);
            queryQueue.Enqueue(query3);

            Assert.Equal(0, queryQueue.GetCurrentIntervalQueriesRanCount());
        }
    }
}
