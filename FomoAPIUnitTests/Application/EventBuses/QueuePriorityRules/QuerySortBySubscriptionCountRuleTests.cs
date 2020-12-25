﻿using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Infrastructure.Enums;
using Moq;
using System.Linq;
using Xunit;
using FomoAPI.Application.EventBuses.QueryContexts;
using System.Threading.Tasks;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;

namespace FomoAPIUnitTests.Application.EventBuses.QueuePriorityRules
{
    public class QuerySortBySubscriptionCountRuleTests
    {
        private readonly Mock<IQueryContextFactory> _mockContextFactory;

        private readonly Mock<IQueryContext> _mockContext;

        public QuerySortBySubscriptionCountRuleTests()
        {
            _mockContextFactory = new Mock<IQueryContextFactory>();
            _mockContext = new Mock<IQueryContext>();

            // Always return as stale query result
            _mockContext.Setup(c => c.GetCachedQueryResult(It.IsAny<int>())).Returns(Task.FromResult<StockQueryResult>(null));
        }

        [Fact]
        public async Task Sort_ShouldSortSubscriptionsBySubscriberCountDescending()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);
            var query3 = new TestQuery(3, QueryFunctionType.IntraDay);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query3);
            querySubscriptions.AddSubscriber(query3);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);
            query3.SetMockQueryContext(_mockContext);

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Equal(3, prioritizedQueries.Count);
            Assert.Equal(prioritizedQueries[0], query2);
            Assert.Equal(prioritizedQueries[1], query3);
            Assert.Equal(prioritizedQueries[1], query3);
        }

        [Fact]
        public async Task Sort_ShouldIgnoreSubscriptionsWithNoSubscribers()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.IntraDay);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);

            querySubscriptions.ClearQuery(query2);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }

        [Fact]
        public async Task Sort_ShouldOnlyIncludeSubscriptionsWithStaleData()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.SingleQuote);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);

            _mockContext.Setup(c => c.GetCachedQueryResult(2)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("MSFT", null)));

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }
    }
}
