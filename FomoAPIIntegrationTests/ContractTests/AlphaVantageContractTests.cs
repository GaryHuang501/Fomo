using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers;
using FomoAPI.Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.ContractTests
{
    public class AlphaVantageContractTests: IDisposable
    {
        private readonly MockHttpClientFactory _mockHttpFactory;
        private readonly Mock<IOptionsMonitor<AlphaVantageOptions>> _mockAlphaVantageOptionsAccessor;
        private readonly AlphaVantageParserFactory _parserFactory;
        private readonly Mock<ILogger<AlphaVantageClient>> _mockLogger;

        public AlphaVantageContractTests()
        {
            _mockAlphaVantageOptionsAccessor = new Mock<IOptionsMonitor<AlphaVantageOptions>>();
            _mockAlphaVantageOptionsAccessor.Setup(x => x.CurrentValue).Returns(AppTestSettings.Instance.LiveAlphaVantageOptions);
            _mockHttpFactory = new MockHttpClientFactory(AppTestSettings.Instance.LiveAlphaVantageOptions.Url);
            _parserFactory = new AlphaVantageParserFactory();
            _mockLogger = new Mock<ILogger<AlphaVantageClient>>();
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnSingleQuoteStockData()
        {
            var stockSymbol = "MSFT";
            var alphaVantageClient = new AlphaVantageClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _parserFactory, _mockLogger.Object);

            SingleQuoteQueryResult singleQuoteResult = await alphaVantageClient.GetSingleQuoteData(stockSymbol, "NASDAQ");

            Assert.False(singleQuoteResult.HasError);

            Assert.True(singleQuoteResult.Data.Low >= 0);
            Assert.True(singleQuoteResult.Data.High >= 0);
            Assert.True(singleQuoteResult.Data.Open >= 0);
            Assert.True(singleQuoteResult.Data.Volume >= 0);
            Assert.True(singleQuoteResult.Data.Price >= 0);
            Assert.True(singleQuoteResult.Data.PreviousClose >= 0);
            Assert.True(singleQuoteResult.Data.LastUpdated > new DateTime());
            Assert.True(singleQuoteResult.Data.LastTradingDay > new DateTime());
            Assert.Equal(stockSymbol, singleQuoteResult.Data.Symbol);
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnErrorWhenUnknownSymbol()
        {
            var stockSymbol = "AB1234567890";
            var alphaVantageClient = new AlphaVantageClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _parserFactory, _mockLogger.Object);

            SingleQuoteQueryResult singleQuoteResult = await alphaVantageClient.GetSingleQuoteData(stockSymbol, "NYSE");

            Assert.True(singleQuoteResult.HasError);
            Assert.False(string.IsNullOrEmpty(singleQuoteResult.ErrorMessage));
        }

        [Fact]
        public async Task GetSearchedTickers_ShouldReturnMatchingSymbols()
        {
            var stockSymbol = "MSFT";
            var alphaVantageClient = new AlphaVantageClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _parserFactory, _mockLogger.Object);

            IEnumerable<SymbolSearchResult> searchResults = await alphaVantageClient.GetSearchedTickers(stockSymbol);

            Assert.True(searchResults.Count() > 1);

            Assert.Equal(stockSymbol, searchResults.First().Symbol);

            SymbolSearchResult msftSymbol = searchResults.First();

            Assert.Contains("Microsoft", msftSymbol.FullName);
            Assert.Equal(1.0000m, msftSymbol.Match);
        }

        public void Dispose()
        {
            _mockHttpFactory.Client.Dispose();
        }
    }
}
