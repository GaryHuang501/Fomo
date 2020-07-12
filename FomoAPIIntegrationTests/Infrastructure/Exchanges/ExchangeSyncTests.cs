using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Infrastructure.Exchanges
{
    public class ExchangeSyncTests : IAsyncLifetime
    {
        private ExchangeSync _sync; 
        
        private ExchangeSyncRepository _syncRepo;

        private SymbolRepository _symbolRepository;

        private string _connectionString = TestAppSettings.Instance.TestDBConnectionString;

        public ExchangeSyncTests()
        {
        }

        public async Task InitializeAsync()
        {
            var mockDbOptions = new Mock<IOptionsMonitor<DbOptions>>();
            mockDbOptions.Setup(x => x.CurrentValue).Returns(new DbOptions
            {
                ConnectionString = TestAppSettings.Instance.TestDBConnectionString,
                DefaultBulkCopyBatchSize = TestAppSettings.Instance.DefaultBulkCopyBatchSize
            });

            var nasdaqLogger = new Mock<ILogger<NasdaqParser>>();
            var exchangeClientLogger = new Mock<ILogger<ExchangeClient>>();

            // Setup dependencies
            _syncRepo = new ExchangeSyncRepository(mockDbOptions.Object);
            _symbolRepository = new SymbolRepository(mockDbOptions.Object);

            string nasdaqDownloadUrl = (await _syncRepo.GetSyncSettings()).Url;

            // Stub FTP so we use downloaded file from disk instead of every test run
            var stubFTPClient = new StubNasdaqFtpClient(nasdaqDownloadUrl);

            var exchangeClient = new ExchangeClient(
                            ftpClient: stubFTPClient,
                            parser: new NasdaqParser(nasdaqLogger.Object),
                            logger: exchangeClientLogger.Object);

            var changesetFactory = new ExchangeSyncChangesetsFactory();

            _sync = new ExchangeSync(exchangeClient, _syncRepo, changesetFactory, new Mock<ILogger<ExchangeSync>>().Object);

            await PrepareDB();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// At the start of every test clear the table and resync the symbols table
        /// </summary>
        /// <returns>Task</returns>
        private async Task PrepareDB()
        {
            await ClearSavedData();

            // Disable sync changes threshold since we just cleared the entire table
            await ToggleSyncThreshold(false);
            await _sync.Sync();
            await ToggleSyncThreshold(true);
        }

        [Fact]
        public async Task Should_Sync_Database_With_Exchange()
        {
            int symbolCountFirstSync = await GetSymbolCount();
            Assert.True(symbolCountFirstSync > 8000);

            // Check for major tickers
            var jpmSymbol = (await _symbolRepository.GetSymbol("JPM"));
            var msftSymbol = (await _symbolRepository.GetSymbol("MSFT"));

            Assert.Equal("JPM", jpmSymbol.Ticker);
            Assert.Equal("NYSE", jpmSymbol.ExchangeName);
            Assert.False(jpmSymbol.Delisted);
            Assert.Contains("JP Morgan Chase & Co", jpmSymbol.FullName);

            Assert.Equal("MSFT", msftSymbol.Ticker);
            Assert.Equal("NASDAQ", msftSymbol.ExchangeName);
            Assert.False(msftSymbol.Delisted);
            Assert.Contains("Microsoft Corporation", msftSymbol.FullName);

            var newSymbolSyncHistory = await GetLatestSyncHistory();
            Assert.Equal(nameof(NewSymbolChangeset), newSymbolSyncHistory.ActionName);
            Assert.Equal(symbolCountFirstSync, newSymbolSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_AddNewSymbols_WhenExistingData()
        {
            int symbolCountFirstSync = await GetSymbolCount();
            var jpmSymbol = (await _symbolRepository.GetSymbol("JPM"));
            var msftSymbol = (await _symbolRepository.GetSymbol("MSFT"));

            await DeleteSymbol(jpmSymbol.Ticker, jpmSymbol.ExchangeId);
            await DeleteSymbol(msftSymbol.Ticker, msftSymbol.ExchangeId);

            int symbolCountAfterDelete = await GetSymbolCount();

            Assert.Equal(symbolCountFirstSync - 2, symbolCountAfterDelete);

            await ToggleSyncThreshold(true);
            await _sync.Sync();

            var readdSyncHistory = await GetLatestSyncHistory();
            Assert.Equal(nameof(NewSymbolChangeset), readdSyncHistory.ActionName);
            Assert.Equal(2, readdSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_DelistSymbols_WhenSymbolNotInExchangeData()
        {
            var testSymbol1 = new Symbol
            {
                ExchangeId = ExchangeType.NASDAQ.Id,
                FullName = "Test Symbol 1",
                Ticker = "Test@1",
            };

            var testSymbol2 = new Symbol
            {
                ExchangeId = ExchangeType.NASDAQ.Id,
                FullName = "Test Symbol 2",
                Ticker = "Test@2",
            };

            await _syncRepo.AddSymbols(new List<Symbol> { testSymbol1, testSymbol2 });
            await _sync.Sync();

            var test1Symbol = (await _symbolRepository.GetSymbol("Test@1"));
            var test2Symbol = (await _symbolRepository.GetSymbol("Test@2"));

            Assert.True(test1Symbol.Delisted);
            Assert.True(test2Symbol.Delisted);

            var delistSyncHistory = await GetLatestSyncHistory();
            Assert.Equal(nameof(SymbolDelistChangeset), delistSyncHistory.ActionName);
            Assert.Equal(2, delistSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_UpdateSymbolDetails_WhenSymbolFullNameChanged()
        {
            await UpdateSymbolFullName("SNAP", ExchangeType.NYSE.Id, "FAKE");
            await UpdateSymbolFullName("DKNG", ExchangeType.NASDAQ.Id, "FAKE");
            await _sync.Sync();

            var snapSymbol = (await _symbolRepository.GetSymbol("SNAP"));
            var dkngSymbol = (await _symbolRepository.GetSymbol("DKNG"));

            Assert.DoesNotContain("FAKE", snapSymbol.FullName);
            Assert.DoesNotContain("FAKE", dkngSymbol.FullName);

            var updateSyncHistory = await GetLatestSyncHistory();
            Assert.Equal(nameof(SymbolDetailsChangeset), updateSyncHistory.ActionName);
            Assert.Equal(2, updateSyncHistory.SymbolsChanged);
        }
   
        [Fact]
        public async Task Should_ThrowException_WhenThresholdExeeded()
        {
            await ClearSavedData();
            await ToggleSyncThreshold(true);
            await Assert.ThrowsAsync<ExchangeSyncException>(async () => await _sync.Sync());
        }


        private async Task<int> GetSymbolCount()
        {
            var sql = @"SELECT COUNT(Id) FROM Symbol";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(sql);     
        }

        private async Task DeleteSymbol(string ticker, int exchangeId)
        {
            var sql = @"DELETE FROM Symbol WHERE Ticker = @ticker AND ExchangeId = @exchangeId";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { ticker, exchangeId});
        }

        private async Task ClearSavedData()
        {
            var symbolSql = @"DELETE FROM Symbol";
            var syncHistorySql = @"DELETE FROM ExchangeSyncHistory";

            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(symbolSql);
            await connection.ExecuteAsync(syncHistorySql);
        }

        private async Task<ExchangeSyncHistory> GetLatestSyncHistory()
        {
            var sql = @"SELECT TOP 1 * FROM ExchangeSyncHistory ORDER BY DateCreated DESC";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<ExchangeSyncHistory>(sql);
        }

        private async Task ToggleSyncThreshold(bool enable)
        {
            var sql = @"UPDATE ExchangeSyncSetting SET DisableThresholds = @disable";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new{ disable = !enable});
        }

        private async Task UpdateSymbolFullName(string ticker, int exchangeId, string fullName)
        {
            var sql = @"UPDATE Symbol SET FullName = @fullName WHERE Ticker = @ticker AND ExchangeId = @exchangeId";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { ticker, exchangeId, fullName });
        }
    }
}
