using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Exchanges;
using FomoAPI.Infrastructure.Repositories;
using FomoAPIIntegrationTests.Fixtures;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    public class ExchangeSyncTests : IAsyncLifetime
    {
        private ExchangeSync _sync; 
        
        private ExchangeSyncRepository _syncRepo;

        private SymbolRepository _symbolRepo;

        private readonly string _connectionString = AppTestSettings.Instance.TestDBConnectionString;

        public ExchangeSyncTests()
        {
        }

        public async Task InitializeAsync()
        {
            var cleanFixture = new CleanDBFixture();
            var exchangeSyncSetupFixture = new ExchangeSyncSetupFixture();

            await cleanFixture.InitializeAsync();
            await exchangeSyncSetupFixture.InitializeAsync();

            _sync = exchangeSyncSetupFixture.ExchangeSync;
            _syncRepo = exchangeSyncSetupFixture.ExchangeSyncRepo;
            _symbolRepo = exchangeSyncSetupFixture.SymbolRepo;
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Should_Sync_Database_With_Exchange()
        {
            int symbolCountFirstSync = await GetSymbolCount();
            Assert.True(symbolCountFirstSync > 8000);

            // Check for major tickers
            var jpmSymbol = await _symbolRepo.GetSymbol("JPM");
            var msftSymbol = await _symbolRepo.GetSymbol("MSFT");

            Assert.Equal("JPM", jpmSymbol.Ticker);
            Assert.Equal("NYSE", jpmSymbol.ExchangeName);
            Assert.False(jpmSymbol.Delisted);
            Assert.Contains("JP Morgan Chase & Co", jpmSymbol.FullName);

            Assert.Equal("MSFT", msftSymbol.Ticker);
            Assert.Equal("NASDAQ", msftSymbol.ExchangeName);
            Assert.False(msftSymbol.Delisted);
            Assert.Contains("Microsoft Corporation", msftSymbol.FullName);

            var newSymbolSyncHistory = (await GetSyncHistory(new DateTime(2000, 1, 1)))
 .                  SingleOrDefault(hist => hist.ActionName == nameof(NewSymbolChangeset));

            Assert.NotNull(newSymbolSyncHistory);
        }

        [Fact]
        public async Task Should_AddNewSymbols_WhenExistingData()
        {
            int symbolCountFirstSync = await GetSymbolCount();
            var jpmSymbol = await _symbolRepo.GetSymbol("JPM");
            var msftSymbol = await _symbolRepo.GetSymbol("MSFT");

            await DeleteSymbol(jpmSymbol.Ticker, jpmSymbol.ExchangeId);
            await DeleteSymbol(msftSymbol.Ticker, msftSymbol.ExchangeId);

            int symbolCountAfterDelete = await GetSymbolCount();

            Assert.Equal(symbolCountFirstSync - 2, symbolCountAfterDelete);

            await ToggleSyncThreshold(true);
            var startDate = DateTime.Now;
            await _sync.Sync();

            var histories = await GetSyncHistory(startDate);
            var readdSyncHistory = histories.SingleOrDefault(hist => hist.ActionName == nameof(NewSymbolChangeset));

            Assert.NotNull(readdSyncHistory);
            Assert.Equal(2, readdSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_DelistSymbols_WhenSymbolNotInExchangeData()
        {
            var testSymbol1 = new InsertSymbolAction("Test@1", ExchangeType.NASDAQ.Id, "Test Symbol 1", false);
            var testSymbol2 = new InsertSymbolAction("Test@2", ExchangeType.NYSE.Id, "Test Symbol 2", false);

            await _syncRepo.AddSymbols(new List<InsertSymbolAction> { testSymbol1, testSymbol2 });
            var startDate = DateTime.Now;
            await _sync.Sync();

            var test1Symbol = await _symbolRepo.GetSymbol("Test@1");
            var test2Symbol = await _symbolRepo.GetSymbol("Test@2");

            Assert.True(test1Symbol.Delisted);
            Assert.True(test2Symbol.Delisted);

            var histories = await GetSyncHistory(startDate);
            var delistSyncHistory = histories.SingleOrDefault(hist => hist.ActionName == nameof(SymbolDelistChangeset));

            Assert.NotNull(delistSyncHistory);
            Assert.Equal(2, delistSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_UpdateSymbolDetails_WhenSymbolFullNameChanged()
        {
            await UpdateSymbolFullName("SNAP", ExchangeType.NYSE.Id, "FAKE");
            await UpdateSymbolFullName("DKNG", ExchangeType.NASDAQ.Id, "FAKE");

            var startDate = DateTime.Now;
            await _sync.Sync();

            var snapSymbol = await _symbolRepo.GetSymbol("SNAP");
            var dkngSymbol = await _symbolRepo.GetSymbol("DKNG");

            Assert.DoesNotContain("FAKE", snapSymbol.FullName);
            Assert.DoesNotContain("FAKE", dkngSymbol.FullName);

            var histories = await GetSyncHistory(startDate);
            var updateSyncHistory = histories.SingleOrDefault(hist => hist.ActionName == nameof(SymbolDetailsChangeset));

            Assert.NotNull(updateSyncHistory);
            Assert.Equal(2, updateSyncHistory.SymbolsChanged);
        }

        [Fact]
        public async Task Should_ThrowException_WhenThresholdEnabledAndZeroSymbols()
        {
            await ClearSavedData();
            await ToggleSyncThreshold(true);
            await Assert.ThrowsAsync<ExchangeSyncException>(async () => await _sync.Sync());
        }

        [Fact]
        public async Task Should_ThrowException_WhenThresholdExeeded()
        {
            int currentSymbolCount = await GetSymbolCount();
            int deleteCountToKeep100Symbols = currentSymbolCount - 100;
            await DeleteRandomSymbols(deleteCountToKeep100Symbols);
            await ToggleSyncThreshold(true);
            await Assert.ThrowsAsync<ExchangeSyncException>(async () => await _sync.Sync());
        }

        private async Task ToggleSyncThreshold(bool enable)
        {
            var sql = @"UPDATE ExchangeSyncSetting SET DisableThresholds = @disable";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { disable = !enable });
        }

        private async Task<int> GetSymbolCount()
        {
            var sql = @"SELECT COUNT(Id) FROM Symbol";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteScalarAsync<int>(sql);     
        }

        private async Task DeleteRandomSymbols(int count)
        {
            var sql = @"DELETE TOP(@count) FROM Symbol";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { count});
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

        private async Task<IEnumerable<ExchangeSyncHistory>> GetSyncHistory(DateTime startDate)
        {
            var sql = @"SELECT * FROM ExchangeSyncHistory WHERE DateCreated > @startDate ORDER BY DateCreated DESC";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ExchangeSyncHistory>(sql, new { startDate });
        }

        private async Task UpdateSymbolFullName(string ticker, int exchangeId, string fullName)
        {
            var sql = @"UPDATE Symbol SET FullName = @fullName WHERE Ticker = @ticker AND ExchangeId = @exchangeId";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { ticker, exchangeId, fullName });
        }
    }
}
