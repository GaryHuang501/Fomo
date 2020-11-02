using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using System;
using System.Linq;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QuerySubscriptionTests
    {
        [Fact]
        public void GetSubscriptionInfos_ReturnsCountZero_WhenZeroQueries()
        {
            var querySubscription = new QuerySubscriptions();
            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Empty(querySubscriptionInfos);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetSubscriptionInfos_ReturnsSubscriberCountEqualTimesAdded(int timesAdded)
        {
            var querySubscription = new QuerySubscriptions();

            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            for(var i = 0; i < timesAdded; i++)
            {
                querySubscription.AddSubscriber(query);
            }

            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Single(querySubscriptionInfos);
            Assert.Equal(timesAdded, querySubscriptionInfos.Single().SubscriberCount);
            Assert.Equal(query, querySubscriptionInfos.Single().Query);
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(3, 2)]
        public void GetSubscriptionInfos_ReturnsSubscriberCountEqualTimesAddedAndSubtracted_WhenSingleQuery(int timesAdded, int timesRemoved)
        {
            var querySubscription = new QuerySubscriptions();

            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            for (var i = 0; i < timesAdded; i++)
            {
                querySubscription.AddSubscriber(query);
            }

            for (var i = 0; i < timesRemoved; i++)
            {
                querySubscription.RemoveSubscriber(query);
            }

            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();
            var expectedCount = Math.Abs(timesAdded - timesRemoved);

            Assert.Single(querySubscriptionInfos);
            Assert.Equal(expectedCount, querySubscriptionInfos.Single().SubscriberCount);
            Assert.Equal(query, querySubscriptionInfos.Single().Query);
        }

        [Fact]
        public void GetSubscriptionInfos_SubscriberCountNeverLessThanZero_WhenSubscribersRemoved()
        {
            var querySubscription = new QuerySubscriptions();

            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            querySubscription.AddSubscriber(query);

            querySubscription.RemoveSubscriber(query);
            querySubscription.RemoveSubscriber(query);
            querySubscription.RemoveSubscriber(query);

            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Single(querySubscriptionInfos);
            Assert.Equal(0, querySubscriptionInfos.Single().SubscriberCount);
            Assert.Equal(query, querySubscriptionInfos.Single().Query);
        }

        [Fact]
        public void GetSubscriptionInfos_ReturnsSubscriberCountEqualsTimesAdded_WhenMultipleQueries()
        {
            var querySubscription = new QuerySubscriptions();

            var queryCount = 2;
            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            var queryCount2 = 3;
            var query2 = new AlphaVantageSingleQuoteQuery("TSLA");

            for (var i = 0; i < queryCount; i++)
            {
                querySubscription.AddSubscriber(query);
            }

            for (var i = 0; i < queryCount2; i++)
            {
                querySubscription.AddSubscriber(query2);
            }

            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Equal(2, querySubscriptionInfos.Count());

            var subcribedQuery = querySubscriptionInfos.First(x => x.Query == query);
            var subcribedQuery2 = querySubscriptionInfos.First(x => x.Query == query2);

            Assert.Equal(queryCount, subcribedQuery.SubscriberCount);
            Assert.Equal(queryCount2, subcribedQuery2.SubscriberCount);
        }

        [Fact]
        public void GetSubscriptionInfos_ReturnsSubscriberCountEqualsTimesAddedAndSubtracted_MultipleQueries()
        {
            var querySubscription = new QuerySubscriptions();

            var queryCount = 2;
            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            var queryCount2 = 3;
            var query2 = new AlphaVantageSingleQuoteQuery("TSLA");

            for (var i = 0; i < queryCount; i++)
            {
                querySubscription.AddSubscriber(query);
            }

            for (var i = 0; i < queryCount2; i++)
            {
                querySubscription.AddSubscriber(query2);
            }

            querySubscription.RemoveSubscriber(query);
            querySubscription.RemoveSubscriber(query2);

            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Equal(2, querySubscriptionInfos.Count());

            var subcribedQuery = querySubscriptionInfos.First(x => x.Query == query);
            var subcribedQuery2 = querySubscriptionInfos.First(x => x.Query == query2);

            Assert.Equal(1, subcribedQuery.SubscriberCount);
            Assert.Equal(2, subcribedQuery2.SubscriberCount);
        }

        [Fact]
        public void GetSubscriptionInfos_ReturnsZeroSubscriberCount_WhenQueryCleared()
        {
            var querySubscription = new QuerySubscriptions();

            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            querySubscription.AddSubscriber(query);
            querySubscription.ClearQuery(query);
            var querySubscriptionInfos = querySubscription.GetSubscriptionInfos();

            Assert.Empty(querySubscriptionInfos);
        }
    }
}
