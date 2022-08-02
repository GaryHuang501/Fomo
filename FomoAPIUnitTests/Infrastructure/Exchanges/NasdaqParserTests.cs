using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.Exchanges
{
    public class NasdaqParserTests
    {

        private readonly NasdaqParser _parser;
        private readonly Mock<ILogger<NasdaqParser>> _mockLogger;

        private readonly SymbolKey MsftKey = new ("MSFT", ExchangeType.NASDAQ.Id);
        private readonly SymbolKey BaKey = new ("BA", ExchangeType.NYSE.Id);
        private readonly SymbolKey VooKey = new ("VOO", ExchangeType.NYSEARCA.Id);
        private readonly SymbolKey ImoKey = new ("IMO", ExchangeType.NYSEAMERICAN.Id);

        public NasdaqParserTests()
        {
            _mockLogger = new Mock<ILogger<NasdaqParser>>();
            _parser = new NasdaqParser(_mockLogger.Object);

        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldReturnTickerMappedToSymbolInfo_WhenSingleRow()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange|Market Category|ETF|Round Lot Size|Test Issue|Financial Status|CQS Symbol|NASDAQ Symbol|NextShares
                         Y|MSFT|Microsoft Corporation|Q|Q|N|100|N|N||MSFT|N";

            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>());

            Assert.Single(tickerToSymbol);
            Assert.True(tickerToSymbol.ContainsKey(MsftKey));

            DownloadedSymbol symbol = tickerToSymbol[MsftKey];
            Assert.Equal(ExchangeType.NASDAQ.Id, symbol.ExchangeId);
            Assert.Equal("Microsoft Corporation", symbol.FullName);
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldReturnTickerMappedToSymbolInfo_ForEachExchangeType()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange|Market Category|ETF|Round Lot Size|Test Issue|Financial Status|CQS Symbol|NASDAQ Symbol|NextShares
                        Y|BA|Boeing Company|N| |N|100|N||BA|BA|N
                        Y|MSFT|Microsoft Corporation|Q|Q|N|100|N|N||MSFT|N
                        Y|VOO|Vanguard S&P 500 ETF|P||Y|100|N||VOO|VOO|N
                        Y|IMO|Imperial Oil Limited|A| |N|100|N||IMO|IMO|N";

            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>());

            Assert.Equal(4, tickerToSymbol.Count);
            Assert.True(tickerToSymbol.ContainsKey(BaKey)); 
            Assert.True(tickerToSymbol.ContainsKey(MsftKey));
            Assert.True(tickerToSymbol.ContainsKey(VooKey));
            Assert.True(tickerToSymbol.ContainsKey(ImoKey));

            DownloadedSymbol baSymbol = tickerToSymbol[BaKey];
            DownloadedSymbol msftSymbol = tickerToSymbol[MsftKey];
            DownloadedSymbol vooSymbol = tickerToSymbol[VooKey]; 
            DownloadedSymbol imoSymbol = tickerToSymbol[ImoKey];

            Assert.Equal(ExchangeType.NYSE.Id, baSymbol.ExchangeId);
            Assert.Equal("Boeing Company", baSymbol.FullName);

            Assert.Equal(ExchangeType.NASDAQ.Id, msftSymbol.ExchangeId);
            Assert.Equal("Microsoft Corporation", msftSymbol.FullName);

            Assert.Equal(ExchangeType.NYSEARCA.Id, vooSymbol.ExchangeId);
            Assert.Equal("Vanguard S&P 500 ETF", vooSymbol.FullName);

            Assert.Equal(ExchangeType.NYSEAMERICAN.Id, imoSymbol.ExchangeId);
            Assert.Equal("Imperial Oil Limited", imoSymbol.FullName);
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldRemoveTrailingSuffixesFromCompanyName()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange|Market Category|ETF|Round Lot Size|Test Issue|Financial Status|CQS Symbol|NASDAQ Symbol|NextShares
                        Y|BA|Boeing Company - Common Stock|N| |N|100|N||BA|BA|N
                        Y|MSFT|Microsoft Corporation Class A Shares|Q|Q|N|100|N|N||MSFT|N
                        Y|VOO|Vanguard S&P 500 ETF - Shared Funds|P||Y|100|N||VOO|VOO|N
                        Y|IMO|Imperial Oil Limited Preferred Shares|A| |N|100|N||IMO|IMO|N";

            var blackList = new string[] { "Preferred Shares", "-", "Class A Shares", "Preferred Shares"};
            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", blackList);

            Assert.Equal(4, tickerToSymbol.Count);
            Assert.True(tickerToSymbol.ContainsKey(BaKey));
            Assert.True(tickerToSymbol.ContainsKey(MsftKey));
            Assert.True(tickerToSymbol.ContainsKey(VooKey));
            Assert.True(tickerToSymbol.ContainsKey(ImoKey));

            DownloadedSymbol baSymbol = tickerToSymbol[BaKey];
            DownloadedSymbol msftSymbol = tickerToSymbol[MsftKey];
            DownloadedSymbol vooSymbol = tickerToSymbol[VooKey];
            DownloadedSymbol imoSymbol = tickerToSymbol[ImoKey];

            Assert.Equal(ExchangeType.NYSE.Id, baSymbol.ExchangeId);
            Assert.Equal("Boeing Company", baSymbol.FullName);

            Assert.Equal(ExchangeType.NASDAQ.Id, msftSymbol.ExchangeId);
            Assert.Equal("Microsoft Corporation", msftSymbol.FullName);

            Assert.Equal(ExchangeType.NYSEARCA.Id, vooSymbol.ExchangeId);
            Assert.Equal("Vanguard S&P 500 ETF", vooSymbol.FullName);

            Assert.Equal(ExchangeType.NYSEAMERICAN.Id, imoSymbol.ExchangeId);
            Assert.Equal("Imperial Oil Limited", imoSymbol.FullName);
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldTrimData()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange
                         Y| MSFT |   Microsoft Corporation|  Q|Q|N|100|N|N||MSFT|N";

            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>());

            Assert.Single(tickerToSymbol);
            Assert.True(tickerToSymbol.ContainsKey(new SymbolKey("MSFT", ExchangeType.NASDAQ.Id)));

            DownloadedSymbol symbol = tickerToSymbol[MsftKey];
            Assert.Equal(ExchangeType.NASDAQ.Id, symbol.ExchangeId);
            Assert.Equal("Microsoft Corporation", symbol.FullName);
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldReturnTickerMappedToSymbolInfo_RegardlessHeadeOrder()
        {
            var data = @"Symbol|Nasdaq Traded|Security Name|Market Category|ETF|Round Lot Size|Test Issue|Financial Status|CQS Symbol|NASDAQ Symbol|NextShares|Listing Exchange
                         MSFT|Y|Microsoft Corporation|Q|N|100|N|N||MSFT|N|Q";

            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>());

            Assert.Single(tickerToSymbol);
            Assert.True(tickerToSymbol.ContainsKey(new SymbolKey("MSFT", ExchangeType.NASDAQ.Id)));

            DownloadedSymbol symbol = tickerToSymbol[MsftKey];
            Assert.Equal(ExchangeType.NASDAQ.Id, symbol.ExchangeId);
            Assert.Equal("Microsoft Corporation", symbol.FullName);
        }

        [Theory]
        [InlineData("Symbol|")]
        [InlineData("Security Name|")]
        [InlineData("Listing Exchange")]
        public async Task GetTickerToSymbolMap_ShouldThrowException_WhenMissingKeyHeader(string header)
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange
                         Y|MSFT|Microsoft Corporation|Q|";

            data = data.Replace(header, "");

            await Assert.ThrowsAsync<FormatException>(async() => await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>()));
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldThrowException_WhenNoData()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange";

            await Assert.ThrowsAsync<ArgumentException>(async () => await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>()));
        }

        [Fact]
        public async Task GetTickerToSymbolMap_ShouldLogAndIgnoreStock_WhenExchangeNotFound()
        {
            var data = @"Nasdaq Traded|Symbol|Security Name|Listing Exchange|Market Category
                        Y|BA|Boeing Company|N||
                        Y|FAKE|FAKE|X|Q|";

            var tickerToSymbol = await _parser.GetSymbolMap(ToStreamReader(data), "|", Array.Empty<string>());

            Assert.Single(tickerToSymbol);
            Assert.True(tickerToSymbol.ContainsKey(BaKey));
        }

        private StreamReader ToStreamReader(string data)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(data);
            MemoryStream stream = new (byteArray);
            return new StreamReader(stream);
        }

    }
}
