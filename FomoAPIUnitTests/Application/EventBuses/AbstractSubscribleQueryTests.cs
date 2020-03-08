using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class AbstractSubscribleQueryTests
    {
        [Fact]
        public void ShouldInstantiateWithDateAndSymbolAndFunctionType()
        {
            var symbol = "MSFT";

            var query = new TestQuery(QueryFunctionType.SingleQuote, symbol);

            Assert.Equal(symbol, query.Symbol);
            Assert.Equal(QueryFunctionType.SingleQuote, query.FunctionType);
            Assert.True(query.CreateDate > default(DateTime));
        }

        [Fact]
        public void ShouldThrowExceptionIfSymbolIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TestQuery(QueryFunctionType.SingleQuote, null));
        }

        public static IEnumerable<object[]> EqualsTestData =>
         new List<object[]>
         {
                        new object[] { "MSFT", QueryFunctionType.SingleQuote, true },
                        new object[] { "TSLA", QueryFunctionType.SingleQuote, false },
                        new object[] { "MSFT", QueryFunctionType.Monthly, false },
                        new object[] { "TSLA", QueryFunctionType.Monthly, false }
         };

        [Theory]
        [MemberData(nameof(EqualsTestData))]
        public void Equals_ShouldReturnTrue_WhenSymbolAndFunctionTypeEqualAndIsISubscribableQuery(string otherQuerySymbol, QueryFunctionType otherQueryFunctionType, bool expectedResult)
        {
            var query = new TestQuery(functionType: QueryFunctionType.SingleQuote, symbol: "MSFT");
            var query2 = new TestQuery(otherQueryFunctionType, otherQuerySymbol);

            Assert.Equal(expectedResult, query.Equals(query2));
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenOtherObjectIsNotAISubscribableQuery()
        {
            var query = new TestQuery(functionType: QueryFunctionType.SingleQuote, symbol: "MSFT");

            Assert.False(query.Equals(new object()));
        }

        [Theory]
        [MemberData(nameof(EqualsTestData))]
        public void GetHashCode_QueriesShouldReturnSameHashCode_WhenSymbolAndFunctionTypeEqual(string otherQuerySymbol, QueryFunctionType otherQueryFunctionType, bool expectedResult)
        {
            var query = new TestQuery(functionType: QueryFunctionType.SingleQuote, symbol: "MSFT");
            var query2 = new TestQuery(otherQueryFunctionType, otherQuerySymbol);

            Assert.Equal(expectedResult, query.GetHashCode() == query2.GetHashCode());
        }
    }
}
