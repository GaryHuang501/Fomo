using FomoAPI.Infrastructure.AlphaVantage;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage
{
    public class AlphaVantageSingleQuoteQueryTests
    {
        [Fact]
        public void GetParameters_ShouldReturnFunctionAndDataTypeAndSymbol()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageSingleQuoteQuery(symbol);
            var parameters = query.GetParameters();

            Assert.Equal(symbol, parameters["symbol"]);
            Assert.Equal("csv", parameters["datatype"]);
            Assert.Equal("GLOBAL_QUOTE", parameters["function"]);
        }

        [Fact]
        public void Function_ShouldReturnSingleQuoteType()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageSingleQuoteQuery(symbol);

            Assert.Equal(QueryFunctionType.SingleQuote, query.FunctionType);
        }

        [Fact]
        public void DataType_ShouldReturnCsvType()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageSingleQuoteQuery(symbol);

            Assert.Equal(QueryDataType.Csv, query.DataType);
        }

        [Fact]
        public void ShouldInstantiateWithCreateDateAndSymbol()
        {
            string symbol = "TSLA";
            var query = new AlphaVantageSingleQuoteQuery(symbol);

            Assert.Equal(symbol, query.Symbol);
            Assert.NotEqual(new DateTime(), query.CreateDate);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ShouldThrowExceptionWhenSymbolNullOrEmptyForConstructor(string symbol)
        {
            Assert.Throws<ArgumentNullException>(() => new AlphaVantageSingleQuoteQuery(symbol));
        }

    }
}
