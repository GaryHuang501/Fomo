using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Infrastructure.AlphaVantage;
using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Infrastructure.AlphaVantage
{
    public class AlphaVantageContractTests: IDisposable
    {
        private readonly MockHttpClientFactory _mockHttpFactory;
        private readonly Mock<IOptionsMonitor<AlphaVantageOptions>> _mockAlphaVantageOptionsAccessor;
        private readonly AlphaVantageParserFactory _parserFactory;
        private readonly Mock<ILogger<AlphaVantageClient>> _mockLogger;

        private class MockHttpClientFactory : IHttpClientFactory
        {
            public string Url { get; set; }

            public HttpClient Client; 
            public MockHttpClientFactory(string url)
            {
                Url = url;
            }

            public HttpClient CreateClient(string name)
            {
                Client =  new HttpClient()
                {
                    BaseAddress = new Uri(Url)
                };

                return Client;
            }

            public void Dispose()
            {
                Client.Dispose();
            }
        }

        public AlphaVantageContractTests()
        {
            _mockAlphaVantageOptionsAccessor = new Mock<IOptionsMonitor<AlphaVantageOptions>>();
            _mockAlphaVantageOptionsAccessor.Setup(x => x.CurrentValue).Returns(AppSettings.Instance.AlphaVantageOptions);
            _mockHttpFactory = new MockHttpClientFactory(AppSettings.Instance.AlphaVantageOptions.Url);
            _parserFactory = new AlphaVantageParserFactory();
            _mockLogger = new Mock<ILogger<AlphaVantageClient>>();
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnSingleQuoteStockData()
        {
            var stockSymbol = "MSFT";
            var alphaVantageQuery = new AlphaVantageSingleQuoteQuery(stockSymbol);
            var alphaVantageClient = new AlphaVantageClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _parserFactory, _mockLogger.Object);

            var singleQuoteResult = await alphaVantageClient.GetSingleQuoteData(alphaVantageQuery);

            Assert.False(singleQuoteResult.HasError);

            Assert.True(singleQuoteResult.Data.Low >= 0);
            Assert.True(singleQuoteResult.Data.High >= 0);
            Assert.True(singleQuoteResult.Data.Open >= 0);
            Assert.NotEqual(new DateTime(), singleQuoteResult.Data.LastTradingDay);
            Assert.Equal(stockSymbol, singleQuoteResult.Data.Symbol);
        }

        [Fact]
        public async Task GetSingleQuote_ShouldReturnErrorWhenUnknownSymbol()
        {
            var stockSymbol = "AB1234567890";
            var alphaVantageQuery = new AlphaVantageSingleQuoteQuery(stockSymbol);
            var alphaVantageClient = new AlphaVantageClient(_mockHttpFactory, _mockAlphaVantageOptionsAccessor.Object, _parserFactory, _mockLogger.Object);

            var singleQuoteResult = await alphaVantageClient.GetSingleQuoteData(alphaVantageQuery);

            Assert.True(singleQuoteResult.HasError);
            Assert.False(string.IsNullOrEmpty(singleQuoteResult.ErrorMessage));
        }

        public void Dispose()
        {
            _mockHttpFactory.Client.Dispose();
        }
    }
}
