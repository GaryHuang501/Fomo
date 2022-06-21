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

        private readonly Mock<IMarketHours> _mockMarketHours;

        public QueryGetPrioritizedQueriesBySubscriptionCountRuleTests()
        {
            _mockContextFactory = new Mock<IQueryContextFactory>();
            _mockContext = new Mock<IQueryContext>();

            // Always return as stale query result
            _mockContext.Setup(c => c.GetCachedQueryResult(It.IsAny<int>())).Returns(Task.FromResult<StockQueryResult>(null));

            _mockMarketHours = new Mock<IMarketHours>();

            // Make it some time way in the future, so data is considered stale.
            _mockMarketHours.Setup(h => h.TodayEndDateUTC()).Returns(new DateTime(2100, 1, 1));
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

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, _mockMarketHours.Object, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

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

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, _mockMarketHours.Object, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }

        [Fact]
        public async Task GetPrioritizedQueries_ShouldIncludeSubscriptions_WhenStaleData()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.SingleQuote);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);

            var staleStockData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "MSFT",
                    price: 7,
                    change: 0.7m,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow.AddMinutes(-10)
                );

            var alreadyUpdatedStockData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "META",
                    price: 7,
                    change: 0.7m,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow.AddMinutes(10)
                );

            _mockMarketHours.Setup(h => h.TodayEndDateUTC()).Returns(DateTime.UtcNow);

            _mockContext.Setup(c => c.GetCachedQueryResult(1)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("MSFT", staleStockData)));
            _mockContext.Setup(c => c.GetCachedQueryResult(2)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("META", alreadyUpdatedStockData)));

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, _mockMarketHours.Object, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }

        [Fact]
        public async Task GetPrioritizedQueries_ShouldIncludeSubscriptions_WhenNotFoundInCache()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(2, QueryFunctionType.SingleQuote);

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);

            query1.SetMockQueryContext(_mockContext);
            query2.SetMockQueryContext(_mockContext);

            var stockData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "MSFT",
                    price: 7,
                    change: 0.7m,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow
                );

            _mockContext.Setup(c => c.GetCachedQueryResult(2)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("MSFT", stockData)));

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, _mockMarketHours.Object, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Single(prioritizedQueries);
            Assert.Equal(prioritizedQueries[0], query1);
        }

        [Fact]
        public async Task GetPrioritizedQueries_ShouldReturnNoResults_WhenDataUpdatedOutsideMarketHours()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(1, QueryFunctionType.SingleQuote);

            querySubscriptions.AddSubscriber(query1);

            query1.SetMockQueryContext(_mockContext);

            var stockData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "MSFT",
                    price: 7,
                    change: 0.7m,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow.AddMinutes(-100)
                ); ;

            _mockMarketHours.Setup(h => h.TodayEndDateUTC()).Returns(new DateTime(2000, 1, 1));

            _mockContext.Setup(c => c.GetCachedQueryResult(1)).Returns(Task.FromResult<StockQueryResult>(new SingleQuoteQueryResult("MSFT", stockData)));

            var priorityRule = new QuerySubscriptionCountRule(_mockContextFactory.Object, querySubscriptions, _mockMarketHours.Object, (new Mock<ILogger<QuerySubscriptionCountRule>>()).Object);

            var prioritizedQueries = (await priorityRule.GetPrioritizedQueries()).ToList();

            Assert.Empty(prioritizedQueries);
        }
    }
}
