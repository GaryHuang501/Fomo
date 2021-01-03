using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.Services;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Infrastructure.Stocks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Application
{
    public class StockDataServiceTests : IDisposable
    {
        private readonly SingleQuoteCache _cache;
        private readonly StockDataService _service;
        private readonly SingleQuoteData _singleQuoteData;

        private Mock<IStockDataRepository> _stockDataRepository;

        public StockDataServiceTests()
        {
            _stockDataRepository = new Mock<IStockDataRepository>();

            var options = new Mock<IOptionsMonitor<CacheOptions>>();
            options.Setup(o => o.Get(SingleQuoteCache.CacheName)).Returns(new CacheOptions
            {
                CacheExpiryTimeMinutes = 10,
                CacheItemSize = 1,
                CacheSize = 100
            });

            _cache = new SingleQuoteCache(options.Object);
            _service = new StockDataService(_stockDataRepository.Object,
                                            _cache,
                                            new QuerySubscriptions(),
                                            new Mock<ILogger<StockDataService>>().Object);

            _singleQuoteData = new SingleQuoteData(
                    symbolId: 1,
                    ticker: "MSFT",
                    high: 1,
                    low: 2,
                    open: 3,
                    previousClose: 4,
                    volume: 5,
                    change: 6,
                    price: 7,
                    changePercent: 0.9m,
                    lastUpdated: DateTime.UtcNow,
                    lastTradingDay: DateTime.UtcNow
                );
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        [Fact]
        public async Task UpsertSingleQuoteData_ShouldUpsertSingleQuoteDataAndSaveIntoCache()
        {
            _stockDataRepository.Setup(s => s.UpsertSingleQuoteData(It.IsAny<UpsertSingleQuoteData>())).Returns(Task.FromResult(true));
            var query = new SingleQuoteQuery(1);
            var queryResult = new SingleQuoteQueryResult("MSFT", _singleQuoteData);
            await _service.UpsertSingleQuoteData(query, queryResult);

            _stockDataRepository.Verify(s => s.UpsertSingleQuoteData(It.Is<UpsertSingleQuoteData>(u => u.SymbolId == query.SymbolId)));
            _cache.TryGet(query.SymbolId, out SingleQuoteQueryResult cachedResult);

            Assert.Equal(queryResult, cachedResult);
        }

        [Fact]
        public async Task UpsertSingleQuoteData_ShouldNotUpsertSingleQuoteData_WhenQueryResultHasError()
        {
            _stockDataRepository.Setup(s => s.UpsertSingleQuoteData(It.IsAny<UpsertSingleQuoteData>())).Returns(Task.FromResult(true));
            var query = new SingleQuoteQuery(1);
            var queryResult = new SingleQuoteQueryResult("error");
            await _service.UpsertSingleQuoteData(query, queryResult);

            _stockDataRepository.Verify(s => s.UpsertSingleQuoteData(It.IsAny<UpsertSingleQuoteData>()), Times.Never);
            Assert.False(_cache.TryGet(query.SymbolId, out SingleQuoteQueryResult cachedResult));
        }

        [Fact]
        public async Task UpsertSingleQuoteData_ShouldThrowException_WhenSavingFailed()
        {
            _stockDataRepository.Setup(s => s.UpsertSingleQuoteData(It.IsAny<UpsertSingleQuoteData>())).Returns(Task.FromResult(false));
            var query = new SingleQuoteQuery(1);
            var queryResult = new SingleQuoteQueryResult("MSFT", _singleQuoteData);
            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpsertSingleQuoteData(query, queryResult));
        }


        [Fact]
        public async Task GetSingleQuoteData_ShouldGetValueFromCache()
        {
            var query = new SingleQuoteQuery(1);
            var queryResult = new SingleQuoteQueryResult("MSFT", _singleQuoteData);

            _cache.Upsert(query.SymbolId, queryResult);

            StockSingleQuoteDataDTO data = await _service.GetSingleQuoteData(query.SymbolId);

            Assert.Equal(queryResult.Data, data.Data);
            _stockDataRepository.Verify(s => s.GetSingleQuoteData(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldGetValueFromDBAndSaveToCache_WhenNotInCache()
        {
            var query = new SingleQuoteQuery(1);
            var queryResult = new SingleQuoteQueryResult("MSFT", _singleQuoteData);

            _stockDataRepository.Setup(s => s.GetSingleQuoteData(It.IsAny<int>())).Returns(Task.FromResult(_singleQuoteData));

            StockSingleQuoteDataDTO data = await _service.GetSingleQuoteData(query.SymbolId);

            Assert.Equal(queryResult.Data, data.Data);
            _stockDataRepository.Verify(s => s.GetSingleQuoteData(It.IsAny<int>()), Times.Once);
            _cache.TryGet(query.SymbolId, out SingleQuoteQueryResult cachedResult);

            Assert.Equal(queryResult.Data, cachedResult.Data);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnEmptyDataSet_WhenNotInDBOrCache()
        {
            _stockDataRepository.Setup(s => s.GetSingleQuoteData(It.IsAny<int>())).Returns(Task.FromResult<SingleQuoteData>(null));

            StockSingleQuoteDataDTO data = await _service.GetSingleQuoteData(1);

            Assert.Null(data.Data);
            Assert.Null(data.LastUpdated);

        }
    }
}
