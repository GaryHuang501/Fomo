using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage.Parsers
{
    public class StockSingleQuoteDataCsvParserTests
    {
        [Fact]
        public void ParseData_ShouldReturnStockSingleQuoteData_WhenValidCsvData()
        {
            var csvData ="symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent" + "\n" +
                         "MSFT,134.8800,139.1000,136.2800,137.8600,21877723,2019-08-30,138.1200,-0.2600,-0.1882%";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new StockSingleQuoteDataCsvParser();
            var singleQuoteData = parser.ParseData(reader);

            Assert.Equal("MSFT", singleQuoteData.Symbol);
            Assert.Equal(134.8800m, singleQuoteData.Open);
            Assert.Equal(139.1000m, singleQuoteData.High);
            Assert.Equal(136.2800m, singleQuoteData.Low);
            Assert.Equal(137.8600m, singleQuoteData.Price);
            Assert.Equal(21877723, singleQuoteData.Volume);
            Assert.Equal(DateTime.Parse("2019-08-30"), singleQuoteData.LastTradingDay);
            Assert.Equal(138.1200m, singleQuoteData.PreviousClose);
            Assert.Equal(-0.2600m, singleQuoteData.Change);
            Assert.Equal("-0.1882%", singleQuoteData.ChangePercent);
        }

        [Fact]
        public void ParseData_ShouldThrowException_WhenEmptyResponseStream()
        {
            var csvData = string.Empty;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new StockSingleQuoteDataCsvParser();

            Assert.Throws<ArgumentException>(() => parser.ParseData(reader));
        }

        [Fact]
        public void ParseData_ShouldThrowException_WhenNoValuesLineInResponseStream()
        {
            var csvData = "symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new StockSingleQuoteDataCsvParser();

            Assert.Throws<ArgumentException>(() => parser.ParseData(reader));
        }

        [Theory]
        [InlineData("symbol")]
        [InlineData("open")]
        [InlineData("high")]
        [InlineData("low")]
        [InlineData("price")]
        [InlineData("latestDay")]
        [InlineData("volume")]
        [InlineData("previousClose")]
        [InlineData("change")]
        [InlineData("changePercent")]
        public void ParseData_ShouldThrowException_WhenResponseStreamMissingOneDataColumn(string columnName)
        {
            var csvData = "symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent" + "\n" +
                         "MSFT,134.8800,139.1000,136.2800,137.8600,21877723,2019-08-30,138.1200,-0.2600,-0.1882%";

            csvData = csvData.Replace(columnName, string.Empty);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new StockSingleQuoteDataCsvParser();
            Assert.Throws<KeyNotFoundException>(() => parser.ParseData(reader));
        }

    }
}
