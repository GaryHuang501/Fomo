using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage
{
    public class AlphaVantageClientTests
    {
        private Mock<IHttpClientFactory> _mockHttpFactory;
        private Mock<IAlphaVantageDataParserFactory> _mockParserFactory;
        private Mock<IAlphaVantageDataParser<StockSingleQuoteData>>_singleQuoteDataCsvParser;
        private readonly Mock<ILogger<AlphaVantageClient>> _mockLogger;
        private readonly Mock<IOptionsMonitor<AlphaVantageOptions>> _mockAlphaVantageOptionsAccessor;

        public AlphaVantageClientTests()
        {
            _singleQuoteDataCsvParser = new Mock<IAlphaVantageDataParser<StockSingleQuoteData>>();

            _mockParserFactory = new Mock<IAlphaVantageDataParserFactory>();
            _mockParserFactory.Setup(x => x.GetSingleQuoteDataParser()).Returns(_singleQuoteDataCsvParser.Object);

            _mockAlphaVantageOptionsAccessor = new Mock<IOptionsMonitor<AlphaVantageOptions>>();
            _mockAlphaVantageOptionsAccessor.Setup(x => x.CurrentValue).Returns(new AlphaVantageOptions
            {
                Url = "https://InvalidURI890999402832.com",
                ClientName = "Client",
                ApiKey = "Key"
            });
            _mockHttpFactory = new Mock<IHttpClientFactory>();
            _mockLogger = new Mock<ILogger<AlphaVantageClient>>();
        }

        private void SetupMockHttpClient(string content, HttpStatusCode statusCode)
        {

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )

               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = statusCode,
                   Content = new StringContent(content),
               })
               .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object);

            _mockHttpFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnQueryResultWithError_WhenParsingError()
        {
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            var query = new AlphaVantageSingleQuoteQuery("MSFT");
            SetupMockHttpClient("", HttpStatusCode.OK);
            _singleQuoteDataCsvParser.Setup(x => x.ParseData(It.IsAny<StreamReader>())).Throws(new Exception());


            var queryResult = await client.GetSingleQuoteData(query);

            Assert.True(queryResult.HasError);
            Assert.NotNull(queryResult.ErrorMessage);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnQueryResultWithError_WhenErrorSendingRequestWithHttpClient()
        {
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            var query = new AlphaVantageSingleQuoteQuery("MSFT");

            _mockHttpFactory = new Mock<IHttpClientFactory>();

            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .Throws<Exception>();

            var httpClient = new HttpClient(handlerMock.Object);
            _mockHttpFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var queryResult = await client.GetSingleQuoteData(query);

            Assert.True(queryResult.HasError);
            Assert.NotNull(queryResult.ErrorMessage);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnQueryResultWithError_WhenHttpRequestNotOK()
        {
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            var query = new AlphaVantageSingleQuoteQuery("MSFT");
            SetupMockHttpClient("", HttpStatusCode.Unauthorized);

            var queryResult = await client.GetSingleQuoteData(query);

            Assert.True(queryResult.HasError);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnSingleQuoteQueryResultWithoutError_WhenSuccessful()
        {
            var singleQuoteData = new StockSingleQuoteData(
                    symbol: "MSFT",
                    high: 1,
                    low: 2,
                    open: 3,
                    previousClose: 4,
                    volume: 5,
                    change: 6,
                    price: 7,
                    changePercent: "9%",
                    lastTradingDay: DateTime.UtcNow
                );

            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            var query = new AlphaVantageSingleQuoteQuery("MSFT");
            SetupMockHttpClient("mockData", HttpStatusCode.OK);
            _singleQuoteDataCsvParser.Setup(x => x.ParseData(It.IsAny<StreamReader>())).Returns(singleQuoteData);

            var queryResult = await client.GetSingleQuoteData(query);

            Assert.False(queryResult.HasError);
            Assert.Equal("MSFT", queryResult.Data.Symbol);
        }
    }
}
