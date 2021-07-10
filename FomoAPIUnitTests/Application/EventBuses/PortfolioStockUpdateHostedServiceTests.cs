using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.Queries;
using FomoAPI.Application.Services;
using FomoAPI.Domain.Stocks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Application.EventBuses
{
    public class PortfolioStockUpdateHostedServiceTests: IDisposable
    {
        private readonly Mock<ILogger<PortfolioStocksUpdateHostedService>> _mockLogger;
        private readonly Mock<IOptionsMonitor<PortfolioStocksUpdateOptions>> _mockOptionsAccessor;
        private PortfolioStocksUpdateHostedService _hostedService;

        public PortfolioStockUpdateHostedServiceTests()
        {
            _mockLogger = new Mock<ILogger<PortfolioStocksUpdateHostedService>>();
            _mockOptionsAccessor = new Mock<IOptionsMonitor<PortfolioStocksUpdateOptions>>();

            _mockOptionsAccessor.Setup(o => o.CurrentValue).Returns(new PortfolioStocksUpdateOptions
            {
                IntervalSeconds = 10,
                BatchSize = 2
            });
        }

        [Fact]
        public async Task ShouldNotUpdate_WhenCurrentTimeIsInsideMarketHours()
        {
            var mockQuery = new Mock<IPortfolioStocksUpdateQuery>();
            var mockStockDataService = new Mock<IStockDataService>();
            var mockHours = new Mock<IMarketHours>();

            mockHours.Setup(h => h.IsMarketHours(It.IsAny<DateTime>())).Returns(true);

            _hostedService = new PortfolioStocksUpdateHostedService(mockQuery.Object, mockStockDataService.Object, _mockLogger.Object, mockHours.Object, _mockOptionsAccessor.Object);
            await _hostedService.StartAsync(new CancellationToken());

            await Task.Delay(100);

            mockQuery.Verify(q => q.Get(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
        }


        [Fact]
        public async Task ShouldUpdate_WhenCurrentTimeIsOutsideMarketHours()
        {
            var mockQuery = new Mock<IPortfolioStocksUpdateQuery>();
            var mockStockDataService = new Mock<IStockDataService>();
            var mockHours = new Mock<IMarketHours>();

            mockHours.Setup(h => h.IsMarketHours(It.IsAny<DateTime>())).Returns(false);

            _hostedService = new PortfolioStocksUpdateHostedService(mockQuery.Object, mockStockDataService.Object, _mockLogger.Object, mockHours.Object, _mockOptionsAccessor.Object);
            await _hostedService.StartAsync(new CancellationToken());

            await Task.Delay(100);

            mockQuery.Verify(q => q.Get(It.IsAny<int>(), It.IsAny<DateTime>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ShouldSubscribeAnyStalePortfolioSymbols_ByBatches()
        {
            var mockQuery = new Mock<IPortfolioStocksUpdateQuery>();
            var mockStockDataService = new Mock<IStockDataService>();
            var mockHours = new Mock<IMarketHours>();

            mockHours.Setup(h => h.IsMarketHours(It.IsAny<DateTime>())).Returns(false);

            IEnumerable<int> queryResult = new int[] { 1, 2, 3 };
            mockQuery.Setup(q => q.Get(It.IsAny<int>(), It.IsAny<DateTime>())).Returns((int top, DateTime date) => Task.FromResult(queryResult.Take(top)));

            _hostedService = new PortfolioStocksUpdateHostedService(mockQuery.Object, mockStockDataService.Object, _mockLogger.Object, mockHours.Object, _mockOptionsAccessor.Object);
            await _hostedService.StartAsync(new CancellationToken());

            await Task.Delay(100);

            mockStockDataService.Verify(s => s.AddSubscriberToSingleQuote(1));
            mockStockDataService.Verify(s => s.AddSubscriberToSingleQuote(2));
            mockStockDataService.Verify(s => s.AddSubscriberToSingleQuote(3), Times.Never);
        }

        public void Dispose()
        {
            _hostedService.Dispose();
        }
    }
}
