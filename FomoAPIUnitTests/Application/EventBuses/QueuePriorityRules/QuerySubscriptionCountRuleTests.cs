using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Infrastructure.Enums;
using Moq;
using System.Linq;
using Xunit;
using FomoAPI.Application.EventBuses.QueryContexts;
using System.Threading.Tasks;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;
using System;
using FomoAPI.Domain.Stocks;

namespace FomoAPIUnitTests.Application.EventBuses.QueuePriorityRules
{
    public class QueryGetPrioritizedQueriesBySubscriptionCountRuleTests
    {
        private readonly Mock<IQueryContextFactory> _mockContextFactory;

        private readonly Mock<IQueryContext> _mockContext;

        public QueryGetPrioritizedQueriesBySubscriptionCountRuleTests()
        {
            _mockContextFactory = new Mock<IQueryContextFactory>();
            _mockContext = new Mock<IQueryContext>();

            // Always return as stale query result
            _mockContext.Setup(c => c.GetCachedQueryResult(It.IsAny<int>())).Returns(Task.FromResult<StockQueryResult>(null));
        }

        [Fact]
        public async Task GetPrioritizedQueries_ShouldGetPrioritizedQueriesSubscriptionsBySubscriberCountDescending()
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
        public async Task GetPrioritizedQueries_ShouldIgnoreSubscriptionsWithNoSubscribers()
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
        public async Task GetPrioritizedQueries_ShouldOnlyIncludeSubscriptionsWithStaleData_Null()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.SingleQuote);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);

            var stockData = new StockSingleQuoteData(
                    symbol: "MSFT",
                    high: 1,
                    low: 2,
                    open: 3,
                    previousClose: 4,
                    volume: 5,
                    change: 6,
                    price: 7,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow,
                    lastTradingDay: DateTime.UtcNow
                );

            _mockContext.Setup(c => c.GetCachedQueryResult(2)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("MSFT", stockData)));

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }
    }
}
