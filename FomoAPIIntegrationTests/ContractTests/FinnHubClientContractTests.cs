using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Clients.FinnHub;
using FomoAPI.Infrastructure.ConfigurationOptions;
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
    public class FinnHubClientContractTests : IDisposable
    {
        private readonly MockHttpClientFactory _mockHttpFactory;
        private readonly Mock<IOptionsMonitor<FinnHubOptions>> _mockAlphaVantageOptionsAccessor;
        private readonly Mock<ILogger<FinnHubClient>> _mockLogger;

        public FinnHubClientContractTests()
        {
            _mockAlphaVantageOptionsAccessor = new Mock<IOptionsMonitor<FinnHubOptions>>();
            _mockAlphaVantageOptionsAccessor.Setup(x => x.CurrentValue).Returns(AppTestSettings.Instance.FinnHubOptions);
            _mockHttpFactory = new MockHttpClientFactory(AppTestSettings.Instance.FinnHubOptions.Url);
            _mockLogger = new Mock<ILogger<FinnHubClient>>();
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnSingleQuoteStockData()
        {
            var query = new SingleQuoteQuery(-1);
            var ticker = "MSFT";
            var FinnHubClient = new FinnHubClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _mockLogger.Object);

            SingleQuoteQueryResult singleQuoteResult = await FinnHubClient.GetSingleQuoteData(query, ticker, "NASDAQ");

            Assert.False(singleQuoteResult.HasError);

            Assert.True(singleQuoteResult.Data.Price > 0);
            Assert.True(singleQuoteResult.Data.LastUpdated > new DateTime());
            Assert.Equal(ticker, singleQuoteResult.Data.Ticker);
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnErrorWhenUnknownSymbol()
        {
            var query = new SingleQuoteQuery(-1);
            var ticker = "AB1234567890";
            var FinnHubClient = new FinnHubClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _mockLogger.Object);

            SingleQuoteQueryResult singleQuoteResult = await FinnHubClient.GetSingleQuoteData(query, ticker, "NYSE");

            Assert.True(singleQuoteResult.HasError);
            Assert.False(string.IsNullOrEmpty(singleQuoteResult.ErrorMessage));
        }

        [Fact]
        public async Task GetSearchedTickers_ShouldReturnMatchingSymbols()
        {
            var stockSymbol = "MSFT";
            var FinnHubClient = new FinnHubClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _mockLogger.Object);

            IEnumerable<SymbolSearchResult> searchResults = await FinnHubClient.GetSearchedTickers(stockSymbol);

            Assert.True(searchResults.Count() > 0);

            Assert.Equal(stockSymbol, searchResults.First().Ticker);

            SymbolSearchResult msftSymbol = searchResults.First();

            Assert.Contains("microsoft", msftSymbol.FullName.ToLower());
            Assert.Equal(1, msftSymbol.Rank);
        }

        public void Dispose()
        {
            _mockHttpFactory.Client.Dispose();
        }
    }
}
