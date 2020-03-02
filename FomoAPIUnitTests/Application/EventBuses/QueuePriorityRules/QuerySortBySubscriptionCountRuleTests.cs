using FomoAPI.Application.Caches;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Infrastructure.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses.QueuePriorityRules
{
    public class QuerySortBySubscriptionCountRuleTests
    {
        public void Sort_ShouldSortSubscriptionsBySubscriberCountDescending()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");
            var query3 = new TestQuery(QueryFunctionType.IntraDay, "SHOP");

            querySubscriptions.AddSubscriber(query1);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query2);
            querySubscriptions.AddSubscriber(query3);
            querySubscriptions.AddSubscriber(query3);

            var sortRule = new QuerySortBySubscriptionCountRule(new Mock<IQueryResultStore>().Object);

            var sortedQueries = sortRule.Sort(querySubscriptions).ToList();

            Assert.Equal(3, sortedQueries.Count);
            Assert.Equal(sortedQueries[0], query2);
            Assert.Equal(sortedQueries[1], query3);
            Assert.Equal(sortedQueries[1], query3);
        }

        public void Sort_ShouldIgnoreSubscriptionsWithNoSubscribers()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");

            querySubscriptions.AddSubscriber(query1);

            var sortRule = new QuerySortBySubscriptionCountRule(new Mock<IQueryResultStore>().Object);

            var sortedQueries = sortRule.Sort(querySubscriptions).ToList();

            Assert.Single(sortedQueries);
            Assert.Equal(sortedQueries[0], query1);
        }

        public void Sort_ShouldOnlyIncludeSubscriptionsWithStaleData()
        {
            var querySubscriptions = new QuerySubscriptions();

            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");

            querySubscriptions.AddSubscriber(query1);

            var sortRule = new QuerySortBySubscriptionCountRule(new Mock<IQueryResultStore>().Object);

            var sortedQueries = sortRule.Sort(querySubscriptions).ToList();

            Assert.Single(sortedQueries);
            Assert.Equal(sortedQueries[0], query1);
        }
    }
}
