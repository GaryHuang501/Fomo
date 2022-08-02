using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Parsers;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Enums;
using FomoAPIUnitTests.Domain.Stocks.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Infrastructure.AlphaVantage
{
    public class AlphaVantageClientTests
    {
        private Mock<IHttpClientFactory> _mockHttpFactory;
        private readonly Mock<IAlphaVantageDataParserFactory> _mockParserFactory;
        private readonly Mock<IAlphaVantageDataParser<SingleQuoteData>>_singleQuoteDataCsvParser;
        private readonly Mock<ILogger<AlphaVantageClient>> _mockLogger;
        private readonly Mock<IOptionsMonitor<AlphaVantageOptions>> _mockAlphaVantageOptionsAccessor;

        public AlphaVantageClientTests()
        {
            _singleQuoteDataCsvParser = new Mock<IAlphaVantageDataParser<SingleQuoteData>>();

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
            var query = new SingleQuoteQuery(-1);
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            SetupMockHttpClient("", HttpStatusCode.OK);
            _singleQuoteDataCsvParser.Setup(x => x.ParseJson(It.IsAny<int>(), It.IsAny<string>())).Throws(new Exception());

            var queryResult = await client.GetSingleQuoteData(query, "MSFT", "NASDAQ");

            Assert.True(queryResult.HasError);
            Assert.NotNull(queryResult.ErrorMessage);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnQueryResultWithError_WhenErrorSendingRequestWithHttpClient()
        {
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);

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

            var query = new SingleQuoteQuery(-1);
            var queryResult = await client.GetSingleQuoteData(query, "MSFT", "NASDAQ");

            Assert.True(queryResult.HasError);
            Assert.NotNull(queryResult.ErrorMessage);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnQueryResultWithError_WhenHttpRequestNotOK()
        {
            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            SetupMockHttpClient("", HttpStatusCode.Unauthorized);

            var query = new SingleQuoteQuery(-1);
            var queryResult = await client.GetSingleQuoteData(query, "MSFT", "NASDAQ");

            Assert.True(queryResult.HasError);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnSingleQuoteQueryResultWithoutError_WhenSuccessful()
        {
            var singleQuoteData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "MSFT",
                    price: 7,
                    change: 0.7m,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow
                );

            var client = new AlphaVantageClient(_mockHttpFactory.Object, _mockAlphaVantageOptionsAccessor.Object, _mockParserFactory.Object, _mockLogger.Object);
            SetupMockHttpClient("mockData", HttpStatusCode.OK);
            _singleQuoteDataCsvParser.Setup(x => x.ParseJson(-1, It.IsAny<string>())).Returns(singleQuoteData);

            var query = new SingleQuoteQuery(-1);
            var queryResult = await client.GetSingleQuoteData(query, "MSFT", "NASDAQ");

            Assert.False(queryResult.HasError);
            Assert.Equal("MSFT", queryResult.Data.Ticker);
            Assert.Equal(singleQuoteData.LastUpdated, queryResult.Data.LastUpdated);
        }
    }
}
