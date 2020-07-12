using FomoAPI.Infrastructure;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Infrastructure.Exchanges
{
    public class NasdaqExchangeClientContractTests
    {
        private readonly IExchangeClient _client;

        private readonly SymbolKey MsftKey = new SymbolKey("MSFT", ExchangeType.NASDAQ.Id);
        private readonly SymbolKey JpmKey = new SymbolKey("JPM", ExchangeType.NYSE.Id);
        private readonly SymbolKey SpyKey = new SymbolKey("SPY", ExchangeType.NYSEARCA.Id);
        private readonly SymbolKey ImoKey = new SymbolKey("IMO", ExchangeType.NYSEAMERICAN.Id);

        private readonly ExchangeSyncSetting _exchangeSyncSetting;

        public NasdaqExchangeClientContractTests()
        {
            var mockLogger = new Mock<ILogger<NasdaqParser>>();
            var parser = new NasdaqParser(mockLogger.Object);

            _client = new ExchangeClient(new FtpClient(), parser, new Mock<ILogger<ExchangeClient>>().Object);

            _exchangeSyncSetting = new ExchangeSyncSetting
            {
                Url = "ftp://ftp.nasdaqtrader.com/symboldirectory/nasdaqtraded.txt",
                ClientName = "NASDAQ",
                Delimiter = "|",
                SuffixBlackList = new string[] { "-", "Common Stock" }
            };
        }

        [Fact]
        public async Task Should_DownloadTradedSymbols()
        {
            var tickerToSymbolMap = await _client.GetTradedSymbols(_exchangeSyncSetting);

            Assert.True(tickerToSymbolMap.Count > 8000);

            // Check that major tickers exists at each exchange
            Assert.True(tickerToSymbolMap.ContainsKey(MsftKey));
            Assert.Equal("MSFT", tickerToSymbolMap[MsftKey].Ticker);
            Assert.Contains("Microsoft Corporation", tickerToSymbolMap[MsftKey].FullName);
            Assert.Equal(ExchangeType.NASDAQ.Id, tickerToSymbolMap[MsftKey].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey(JpmKey));
            Assert.Equal("JPM", tickerToSymbolMap[JpmKey].Ticker);
            Assert.Contains("JP Morgan Chase & Co.", tickerToSymbolMap[JpmKey].FullName);
            Assert.Equal(ExchangeType.NYSE.Id, tickerToSymbolMap[JpmKey].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey(SpyKey));
            Assert.Equal("SPY", tickerToSymbolMap[SpyKey].Ticker);
            Assert.Contains("SPDR S&P 500", tickerToSymbolMap[SpyKey].FullName);
            Assert.Equal(ExchangeType.NYSEARCA.Id, tickerToSymbolMap[SpyKey].ExchangeId);

            Assert.True(tickerToSymbolMap.ContainsKey(ImoKey));
            Assert.Equal("IMO", tickerToSymbolMap[ImoKey].Ticker);
            Assert.Contains("Imperial Oil Limited", tickerToSymbolMap[ImoKey].FullName);
            Assert.Equal(ExchangeType.NYSEAMERICAN.Id, tickerToSymbolMap[ImoKey].ExchangeId);
        }
    }
}
