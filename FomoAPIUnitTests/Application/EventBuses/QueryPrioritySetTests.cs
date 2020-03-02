using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Enums;
using System.Linq;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class QueryPrioritySetTests
    {
        [Fact]
        public void IsEmpty_ShouldReturnTrue_WhenNoQueriesAdded()
        {
            var querySet = new QueryPrioritySet();

            Assert.True(querySet.IsEmpty);
        }

        [Fact]
        public void IsEmpty_ShouldReturnFalse_WhenQueryAdded()
        {
            var querySet = new QueryPrioritySet();
            var query = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            querySet.TryAdd(query);

            Assert.False(querySet.IsEmpty);
        }

        [Fact]
        public void Count_ShouldReturnSetSize()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");
            var query3 = new TestQuery(QueryFunctionType.Monthly, "GOOG");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);
            querySet.TryAdd(query3);

            Assert.Equal(3, querySet.Count);
        }

        [Fact]
        public void Take_ShouldReturnTopFirstQueryInFifoOrder_WhenMultipleQueriesAdded()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);

            var peekedQueries = querySet.Take(1).ToList();
            Assert.Equal(query1, peekedQueries[0]);
            Assert.Equal(2, querySet.Count);
        }

        [Fact]
        public void Take_ShouldReturnTopThreeQueryInFifoOrder_WhenMultipleQueriesAdded()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");
            var query3 = new TestQuery(QueryFunctionType.Monthly, "GOOG");
            var query4 = new TestQuery(QueryFunctionType.Monthly, "SHOP");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);
            querySet.TryAdd(query3);
            querySet.TryAdd(query4);

            var peekedQueries = querySet.Take(3).ToList();
            Assert.Equal(query1, peekedQueries[0]);
            Assert.Equal(query2, peekedQueries[1]);
            Assert.Equal(query3, peekedQueries[2]);
            Assert.Equal(4, querySet.Count);
        }

        [Fact]
        public void Take_ShouldReturnAllItems_WhenItemsToTakeExceedsSetSize()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.IntraDay, "TSLA");
            var query3 = new TestQuery(QueryFunctionType.Monthly, "GOOG");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);
            querySet.TryAdd(query3);

            var peekedQueries = querySet.Take(100).ToList();
            Assert.Equal(3, peekedQueries.Count);
            Assert.Equal(3, querySet.Count);
        }

        [Fact]
        public void TryAdd_ShouldReturnTrue_WhenQueryIsAddedSuccessfully()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            bool isSuccessful = querySet.TryAdd(query1);

            Assert.True(isSuccessful);
        }

        [Fact]
        public void TryAdd_ShouldReturnFalseAndNotAddQuery_WhenQueryExists()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            querySet.TryAdd(query1);
            bool isSecondAddSuccessful = querySet.TryAdd(query1);

            Assert.False(isSecondAddSuccessful);
            Assert.Equal(1, querySet.Count);
        }

        [Fact]
        public void Remove_ShouldReturnTrue_WhenSingleSetHasItemRemoved()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            querySet.TryAdd(query1);
            bool IsRemoveSuccessful = querySet.Remove(query1);

            Assert.True(IsRemoveSuccessful);
            Assert.True(querySet.IsEmpty);
        }

        [Fact]
        public void Remove_ShouldBeAbleToRemoveMultipleQueries()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.SingleQuote, "SHOP");
            var query3 = new TestQuery(QueryFunctionType.SingleQuote, "AMD");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);
            querySet.TryAdd(query3);

            querySet.Remove(query1);
            querySet.Remove(query2);
            querySet.Remove(query3);

            Assert.True(querySet.IsEmpty);
        }


        [Fact]
        public void Remove_ShouldBeAbleToRemoveMultipleQueriesInAnyOrder()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.SingleQuote, "SHOP");
            var query3 = new TestQuery(QueryFunctionType.SingleQuote, "AMD");

            querySet.TryAdd(query1);
            querySet.TryAdd(query2);
            querySet.TryAdd(query3);

            // Remove middle element and readd 
            // Should return true for add since duplicate should not exist
            bool isRemoveSuccessful = querySet.Remove(query2);
            var duplicateDoesNotExist = querySet.TryAdd(query2);

            Assert.True(isRemoveSuccessful);
            Assert.True(duplicateDoesNotExist);
        }

        [Fact]
        public void Remove_ShouldReturnFalse_WhenSetIsEmpty()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            bool isRemoveSuccessful = querySet.Remove(query1);

            Assert.False(isRemoveSuccessful);
        }

        [Fact]
        public void Remove_ShouldReturnFalse_WhenQueryNotFound()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");
            var query2 = new TestQuery(QueryFunctionType.SingleQuote, "SHOP");

            querySet.TryAdd(query1);
            bool isRemoveSuccessful = querySet.Remove(query2);

            Assert.False(isRemoveSuccessful);
        }

        [Fact]
        public void TryAdd_ShouldReturnTrue_WhenQueryIsAddedAgainAfterItWasRemoved()
        {
            var querySet = new QueryPrioritySet();
            var query1 = new TestQuery(QueryFunctionType.SingleQuote, "MSFT");

            querySet.TryAdd(query1);
            querySet.Remove(query1);
            bool isAddSuccessful = querySet.TryAdd(query1);

            Assert.True(isAddSuccessful);
        }

    }
}
