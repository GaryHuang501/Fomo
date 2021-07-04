using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage
{
    public class AlphaVantageQueryTests
    {
        [Fact]
        public void GetParameters_ShouldReturnFunctionAndDataTypeAndSymbol()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageQuery(QueryFunctionType.SingleQuote, symbol);
            var parameters = query.GetParameters();

            Assert.Equal(symbol, parameters["symbol"]);
            Assert.Equal("json", parameters["datatype"]);
            Assert.Equal("GLOBAL_QUOTE", parameters["function"]);
        }

        [Fact]
        public void Function_ShouldReturnSingleQuoteType()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageQuery(QueryFunctionType.SingleQuote, symbol);

            Assert.Equal(QueryFunctionType.SingleQuote, query.FunctionType);
        }

        [Fact]
        public void DataType_ShouldReturnJSONType()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageQuery(QueryFunctionType.SingleQuote, symbol);

            Assert.Equal(QueryDataType.Json, query.DataType);
        }

        [Fact]
        public void ShouldInstantiateWithCreateDateAndSymbol()
        {
            string symbol = "JPM";
            var query = new AlphaVantageQuery(QueryFunctionType.SingleQuote, symbol);

            Assert.Equal(symbol, query.Ticker);
            Assert.NotEqual(new DateTime(), query.CreateDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldThrowExceptionWhenSymbolNullOrEmptyForConstructor(string symbol)
        {
            Assert.Throws<ArgumentNullException>(() => new AlphaVantageQuery(QueryFunctionType.SingleQuote, symbol));
        }

    }
}
