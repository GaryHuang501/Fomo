using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Domain.Stocks.Queries
{
    public class StockQueryTests
    {
        [Fact]
        public void ShouldInstantiateWithDateAndSymbolAndFunctionType()
        {
            var symbolId = 1;

            var query = new TestQuery(symbolId, QueryFunctionType.SingleQuote);

            Assert.Equal(symbolId, query.SymbolId);
            Assert.Equal(QueryFunctionType.SingleQuote, query.FunctionType);
            Assert.True(query.CreateDate > default(DateTime));
        }

        public static IEnumerable<object[]> EqualsTestData =>
         new List<object[]>
         {
                        new object[] { 1, QueryFunctionType.SingleQuote, true },
                        new object[] { 2, QueryFunctionType.SingleQuote, false },
                        new object[] { 1, QueryFunctionType.Monthly, false },
                        new object[] { 3, QueryFunctionType.Monthly, false }
         };

        [Theory]
        [MemberData(nameof(EqualsTestData))]
        public void Equals_ShouldReturnTrue_WhenSymbolAndFunctionTypeEqual(int otherQuerySymbolId, QueryFunctionType otherQueryFunctionType, bool expectedResult)
        {
            var query = new TestQuery(symbolId: 1, functionType: QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(otherQuerySymbolId, otherQueryFunctionType);

            Assert.Equal(expectedResult, query.Equals(query2));
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenTypeDifferent()
        {
            var query = new TestQuery(symbolId: 1, functionType: QueryFunctionType.SingleQuote);

            Assert.False(query.Equals(new { SymbolId = 1, FunctionType = QueryFunctionType.SingleQuote }));
        }

        [Theory]
        [MemberData(nameof(EqualsTestData))]
        public void GetHashCode_QueriesShouldReturnSameHashCode_WhenEqual(int otherQuerySymbolId, QueryFunctionType otherQueryFunctionType, bool expectedResult)
        {
            var query = new TestQuery(symbolId: 1, functionType: QueryFunctionType.SingleQuote);
            var query2 = new TestQuery(otherQuerySymbolId, otherQueryFunctionType);

            Assert.Equal(expectedResult, query.GetHashCode() == query2.GetHashCode());
        }
    }
}
