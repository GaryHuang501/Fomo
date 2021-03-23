using Dapper;
using FomoAPI.Application.DTOs;
using FomoAPI.Infrastructure.Enums;
using FomoAPIIntegrationTests.Fixtures;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    /// <summary>
    /// Tests for portfolio to get stock data and check stock queries are properly queued and queried.
    /// </summary>
    /// 
    /// <remarks>
    /// Calls mocked Alphavantage API through postman.
    /// </remarks>
    public class StockQueryTests : IClassFixture<ExchangeSyncSetupFixture>, IClassFixture<DBFixture>, IAsyncLifetime
    {
        private readonly DBFixture _dbFixture;
        private HttpClient _client;
        private FomoApiFixture _fomoApiFixture;

        public StockQueryTests(DBFixture dbFixture)
        {
            _dbFixture = dbFixture;
        }

        public async Task InitializeAsync()
        {
            await RestartServer();
            await ClearSingleQuoteData();
        }

        public Task DisposeAsync()
        {
            _fomoApiFixture.Dispose();
            GC.Collect();

            return Task.CompletedTask;
        }

        private async Task RestartServer()
        {
            if (_fomoApiFixture != null && !_fomoApiFixture.Disposed)
            {
                _fomoApiFixture.Dispose();
                GC.Collect();
            }

            _fomoApiFixture = new FomoApiFixture();
            _fomoApiFixture.CreateServer(null);
            _client = _fomoApiFixture.GetClientNotAuth();
            await Task.Delay(2 * AppTestSettings.Instance.EventBusOptions.RefreshIntervalMS); // To let interval query interval finish.
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnNoData_WhenEmptySymbolIds()
        {
            List<StockSingleQuoteDataDTO> data = await GetSingleQuoteData(new int[] { });

            Assert.Empty(data);

            await Task.Delay(1000);

            int singleQuoteCount = await _dbFixture.Connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM SingleQuoteData;");

            Assert.Equal(0, singleQuoteCount);
        }

        /// <summary>
        /// Tests to check fetching single quote data will return no data since it's the first time
        /// it has been called. This should also cause the query to queue up and eventually executed.
        /// So calling this end point again it will return the data when it has executed.
        /// </summary>
        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnNoDataAndQueueQuery_WhenFirstTime()
        {
            SymbolSearchResultDTO searchResult = await SearchSymbol("TSLA", ExchangeType.NASDAQ);
            List<StockSingleQuoteDataDTO> dataDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            Assert.Single(dataDtos);

            var dataDto = dataDtos[0];

            Assert.Equal(searchResult.SymbolId, dataDto.SymbolId);
            Assert.Null(dataDto.Data);

            var startTime = DateTime.UtcNow;

            while (dataDto.Data == null)
            {
                await CheckTimeOut(startTime);

                List<StockSingleQuoteDataDTO> newData = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
                dataDto = newData[0];
            }

            AssertSingleQuote(dataDto, searchResult.SymbolId);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldReturnDataFromCacheAndDB_WhenItExists()
        {
            // First call should return no data since it does not exist.
            SymbolSearchResultDTO searchResult = await SearchSymbol("TSLA", ExchangeType.NASDAQ);
            List<StockSingleQuoteDataDTO> dataDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            Assert.Single(dataDtos);

            var dataDto = dataDtos[0];

            Assert.Equal(searchResult.SymbolId, dataDto.SymbolId);
            Assert.Null(dataDto.Data);

            var startTime = DateTime.UtcNow;

            // Wait for query to execute and until there is data
            while (dataDto.Data == null)
            {
                await CheckTimeOut(startTime);

                List<StockSingleQuoteDataDTO> newData = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
                dataDto = newData[0];
            }

            AssertSingleQuote(dataDto, searchResult.SymbolId);

            // Next update the LastUpdated date of the single quote data entry in the database.
            // however when this single quote data is fetched it the date should not match since
            // the database is updated but not cache, which is where it will be fetched from.
            var modifiedDate = new DateTime(2000, 1, 1); ;
            await _dbFixture.Connection.ExecuteAsync($"UPDATE SingleQuoteData SET LastUpdated = '{modifiedDate}'");

            var cacheDataDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            Assert.Single(cacheDataDtos);

            var cacheDataDto = cacheDataDtos[0];
            AssertSingleQuote(cacheDataDto, searchResult.SymbolId);
            Assert.NotEqual(cacheDataDto.LastUpdated.Value.ToString("yyyy-mm-dd"), modifiedDate.ToString("yyyy-mm-dd"));

            // Restarting server will clear cache so we should get the updated DB value.
            await RestartServer();

            var dbDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            Assert.Single(dataDtos);

            var dbDto = dbDtos[0];
            AssertSingleQuote(dbDto, searchResult.SymbolId);

            Assert.Equal(dbDto.LastUpdated.Value.ToString("yyyy-mm-dd"), modifiedDate.ToString("yyyy-mm-dd"));

            // Update the DB entry again. Getting the data should get the cache version so it will be unaffected.
            await _dbFixture.Connection.ExecuteAsync($"UPDATE SingleQuoteData SET LastUpdated = '2001-01-01'");
            var secondCachedDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            var secondCachedDto = secondCachedDtos[0];
            Assert.Equal(dbDto.LastUpdated, secondCachedDto.LastUpdated);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldUpdateQueryResults_WhenResultsFromDBStale()
        {
            // Trigger the query so it is saved into the database.
            SymbolSearchResultDTO searchResult = await SearchSymbol("TSLA", ExchangeType.NASDAQ);
            List<StockSingleQuoteDataDTO> dataDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            Assert.Single(dataDtos);

            var dataDto = dataDtos[0];

            Assert.Equal(searchResult.SymbolId, dataDto.SymbolId);
            Assert.Null(dataDto.Data);

            var startTime = DateTime.UtcNow;

            // Wait for query to execute and until there is data
            while (dataDto.Data == null)
            {
                await CheckTimeOut(startTime);

                List<StockSingleQuoteDataDTO> newData = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
                dataDto = newData[0];
            }

            AssertSingleQuote(dataDto, searchResult.SymbolId);

            // Make the data stale
            var modifiedDate = new DateTime(2000, 1, 1); ;
            await _dbFixture.Connection.ExecuteAsync($"UPDATE SingleQuoteData SET LastUpdated = '{modifiedDate}'");

            //reset the server so it will update the cache with the stale value.
            // System should detect the stale data and trigger an update.
            await RestartServer();

            var dbDataDtos = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
            var dbDataDto = dbDataDtos[0];

            // Wait for query to execute and until there is data
            while (dbDataDto.LastUpdated <= modifiedDate)
            {
                await CheckTimeOut(startTime);

                List<StockSingleQuoteDataDTO> newData = await GetSingleQuoteData(new int[] { searchResult.SymbolId });
                dbDataDto = newData[0];
            }

            Assert.Equal(searchResult.SymbolId, dbDataDto.SymbolId);
            Assert.True(dbDataDto.LastUpdated > DateTime.UtcNow.AddMinutes(-5));
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldQueueQuery_ForEachStock()
        {
            // First call should return no data since it does not exist.
            SymbolSearchResultDTO tslaSearchResult = await SearchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO jpmSearchResult = await SearchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO shopSearchResult = await SearchSymbol("SHOP", ExchangeType.NASDAQ);

            var symbolIdsToGet = new int[]
                                        {
                                            tslaSearchResult.SymbolId,
                                            jpmSearchResult.SymbolId,
                                            shopSearchResult.SymbolId,
                                        };

            List<StockSingleQuoteDataDTO> dataDtos = await GetSingleQuoteData(symbolIdsToGet);
            Assert.Equal(3, dataDtos.Count());

            Assert.All(dataDtos, d => Assert.Null(d.Data));

            var startTime = DateTime.UtcNow;

            // Wait for query to execute until all populated
            while (dataDtos.Count(d => d.Data == null) != 0)
            {
                await CheckTimeOut(startTime);

                dataDtos = await GetSingleQuoteData(symbolIdsToGet);
            }

            AssertSingleQuote(dataDtos[0], tslaSearchResult.SymbolId);
            AssertSingleQuote(dataDtos[1], jpmSearchResult.SymbolId);
            AssertSingleQuote(dataDtos[2], shopSearchResult.SymbolId);
        }

        [Fact]
        public async Task GetSingleQuoteData_ShouldQueueQuery_ForEachStock_WhenTwoIntervals()
        {
            // First call should return no data since it does not exist.
            SymbolSearchResultDTO tslaSearchResult = await SearchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO jpmSearchResult = await SearchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO shopSearchResult = await SearchSymbol("SHOP", ExchangeType.NASDAQ);
            SymbolSearchResultDTO faceBookSearchResult = await SearchSymbol("FB", ExchangeType.NASDAQ);



            var symbolIdsToGet = new int[]
                                        {
                                            tslaSearchResult.SymbolId,
                                            jpmSearchResult.SymbolId,
                                            shopSearchResult.SymbolId,
                                            faceBookSearchResult.SymbolId
                                        };

            if (AppTestSettings.Instance.EventBusOptions.MaxQueriesPerInterval >= symbolIdsToGet.Length)
            {
                throw new InvalidOperationException("Number of symbols to fetch should be greater than the max per interval.");
            }

            List<StockSingleQuoteDataDTO> dataDtos = await GetSingleQuoteData(symbolIdsToGet);

            Assert.Equal(4, dataDtos.Count());

            Assert.All(dataDtos, d => Assert.Null(d.Data));

            var startTime = DateTime.UtcNow;

            // Wait for query to execute until all populated
            while (dataDtos.Count(d => d.Data == null) != 0)
            {
                await CheckTimeOut(startTime);

                dataDtos = await GetSingleQuoteData(symbolIdsToGet);
            }

            AssertSingleQuote(dataDtos[0], tslaSearchResult.SymbolId);
            AssertSingleQuote(dataDtos[1], jpmSearchResult.SymbolId);
            AssertSingleQuote(dataDtos[2], shopSearchResult.SymbolId);
            AssertSingleQuote(dataDtos[3], faceBookSearchResult.SymbolId);
        }

        private void AssertSingleQuote(StockSingleQuoteDataDTO dataDto, int expectedSymbolId)
        {
            Assert.Equal(expectedSymbolId, dataDto.SymbolId);
            Assert.NotNull(dataDto.Data);
            Assert.True(dataDto.Data.Low >= 0);
            Assert.True(dataDto.Data.High >= 0);
            Assert.True(dataDto.Data.Open >= 0);
            Assert.True(dataDto.Data.Volume >= 0);
            Assert.True(dataDto.Data.Price >= 0);
            Assert.True(dataDto.Data.PreviousClose >= 0);
            Assert.True(dataDto.Data.LastUpdated > new DateTime());
            Assert.True(dataDto.Data.LastTradingDay > new DateTime());
        }


        private async Task ClearSingleQuoteData()
        {
            await _dbFixture.Connection.ExecuteAsync("DELETE FROM SingleQuoteData");
        }

        private async Task CheckTimeOut(DateTime startTime)
        {
            await Task.Delay(100);

            if (DateTime.UtcNow > startTime.AddSeconds(150))
            {
                throw new TimeoutException("Valid data was never returned");
            }
        }

        private async Task<List<StockSingleQuoteDataDTO>> GetSingleQuoteData(int[] symbolIds)
        {
            string singleQuotePath = ApiPath.GetSingleQuoteData(symbolIds);
            var response = await _client.GetAsync(singleQuotePath);
            var data = await response.Content.ReadAsAsync<IEnumerable<StockSingleQuoteDataDTO>>();

            return data.ToList();
        }

        private async Task<SymbolSearchResultDTO> SearchSymbol(string ticker, ExchangeType exchange)
        {
            var response = await _client.GetAsync(ApiPath.SymbolSearch(ticker, 1));
            response.EnsureSuccessStatusCode();
            var searchResultsJson = await response.Content.ReadAsStringAsync();
   
            var settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;
            settings.NullValueHandling = NullValueHandling.Include;

            var searchResults = JsonConvert.DeserializeObject<IEnumerable<SymbolSearchResultDTO>>(searchResultsJson, settings);

            var result = searchResults.Single();

            Assert.Equal(ticker, result.Ticker);

            return searchResults.Single();
        }
    }
}
