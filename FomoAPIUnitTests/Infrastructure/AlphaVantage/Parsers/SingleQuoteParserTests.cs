using FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage.Parsers
{
    public class SingleQuoteParserTests
    {
        [Fact]
        public void ParseCsv_ShouldReturnStockSingleQuoteData_WhenValidCsvData()
        {
            var csvData ="symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent" + "\n" +
                         "MSFT,134.8800,139.1000,136.2800,137.8600,21877723,2019-08-30,138.1200,-0.2600,-0.1882%";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new SingleQuoteParser();
            var singleQuoteData = parser.ParseCsv(-1, reader);

            Assert.Equal("MSFT", singleQuoteData.Ticker);
            Assert.Equal(134.8800m, singleQuoteData.Open);
            Assert.Equal(139.1000m, singleQuoteData.High);
            Assert.Equal(136.2800m, singleQuoteData.Low);
            Assert.Equal(137.8600m, singleQuoteData.Price);
            Assert.Equal(21877723, singleQuoteData.Volume);
            Assert.Equal(DateTime.Parse("2019-08-30"), singleQuoteData.LastTradingDay);
            Assert.Equal(138.1200m, singleQuoteData.PreviousClose);
            Assert.Equal(-0.2600m, singleQuoteData.Change);
            Assert.Equal(-0.1882m, singleQuoteData.ChangePercent);
        }

        [Fact]
        public void ParseCsv_ShouldThrowException_WhenEmptyResponseStream()
        {
            var csvData = string.Empty;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new SingleQuoteParser();

            Assert.Throws<ArgumentException>(() => parser.ParseCsv(-1, reader));
        }

        [Fact]
        public void ParseCsv_ShouldThrowException_WhenNoValuesLineInResponseStream()
        {
            var csvData = "symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent";

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new SingleQuoteParser();

            Assert.Throws<ArgumentException>(() => parser.ParseCsv(-1, reader));
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
        public void ParseCsv_ShouldThrowException_WhenResponseStreamMissingOneDataColumn(string columnName)
        {
            var csvData = "symbol,open,high,low,price,volume,latestDay,previousClose,change,changePercent" + "\n" +
                         "MSFT,134.8800,139.1000,136.2800,137.8600,21877723,2019-08-30,138.1200,-0.2600,-0.1882%";

            csvData = csvData.Replace(columnName, string.Empty);

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(csvData));
            var reader = new StreamReader(stream);
            var parser = new SingleQuoteParser();
            Assert.Throws<KeyNotFoundException>(() => parser.ParseCsv(-1, reader));
        }

    }
}
