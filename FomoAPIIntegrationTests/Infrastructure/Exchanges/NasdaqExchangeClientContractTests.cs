using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Infrastructure;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Infrastructure.Exchanges
{
    public class NasdaqExchangeClientContractTests
    {
        private readonly IExchangeClient _client;

        public NasdaqExchangeClientContractTests()
        {
            var mockLogger = new Mock<ILogger<NasdaqParser>>();
            var options = AppSettings.Instance.NasdaqOptions;
            var mockOptionsAccessor = new Mock<IOptionsMonitor<NasdaqOptions>>();
            mockOptionsAccessor.Setup(x => x.CurrentValue).Returns(options);
            var parser = new NasdaqParser(mockLogger.Object);

            _client = new ExchangeClient(new FtpClient(), mockOptionsAccessor.Object, parser, new Mock<ILogger<ExchangeClient>>().Object);
        }

        [Fact]
        public async Task Should_DownloadTradedSymbols()
        {
            var tickerToSymbolMap = await _client.GetTradedSymbols();

            Assert.True(tickerToSymbolMap.Count > 8000);

            // Check that major tickers exists at each exchange
            Assert.True(tickerToSymbolMap.ContainsKey("MSFT"));
            Assert.Equal("MSFT", tickerToSymbolMap["MSFT"].Ticker);
            Assert.Contains("Microsoft Corporation", tickerToSymbolMap["MSFT"].FullName);
            Assert.Equal(ExchangeType.NASDAQ.Id, tickerToSymbolMap["MSFT"].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey("JPM"));
            Assert.Equal("JPM", tickerToSymbolMap["JPM"].Ticker);
            Assert.Contains("JP Morgan Chase & Co.", tickerToSymbolMap["JPM"].FullName);
            Assert.Equal(ExchangeType.NYSE.Id, tickerToSymbolMap["JPM"].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey("SPY"));
            Assert.Equal("SPY", tickerToSymbolMap["SPY"].Ticker);
            Assert.Contains("SPDR S&P 500", tickerToSymbolMap["SPY"].FullName);
            Assert.Equal(ExchangeType.NYSEARCA.Id, tickerToSymbolMap["SPY"].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey("IMO"));
            Assert.Equal("IMO", tickerToSymbolMap["IMO"].Ticker);
            Assert.Contains("Imperial Oil Limited", tickerToSymbolMap["IMO"].FullName);
            Assert.Equal(ExchangeType.NYSEAMERICAN.Id, tickerToSymbolMap["IMO"].ExchangeId);
        }
    }
}
