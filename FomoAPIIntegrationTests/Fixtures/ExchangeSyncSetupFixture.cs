using Dapper;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Exchanges;
using FomoAPI.Infrastructure.Repositories;
using FomoAPIIntegrationTests.Scenarios;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Fixtures
{
    /// <summary>
    /// Sync DB with exchange before running tests for a class.
    /// Only runs if symbol data is missing.
    /// </summary>
    public class ExchangeSyncSetupFixture : IAsyncLifetime
    {
        public ExchangeSync ExchangeSync { get; private set; }
        public ExchangeSyncRepository ExchangeSyncRepo { get; private set; }
        public SymbolRepository SymbolRepo { get; private set; }

        private readonly SqlConnection _connection;

        private readonly string _connectionString = AppTestSettings.Instance.TestDBConnectionString;

        public ExchangeSyncSetupFixture()
        {
            _connection = new SqlConnection(AppTestSettings.Instance.TestDBConnectionString);
        }

        public async Task InitializeAsync()
        {
            var mockDbOptions = new Mock<IOptionsMonitor<DbOptions>>();
            mockDbOptions.Setup(x => x.CurrentValue).Returns(new DbOptions
            {
                ConnectionString = AppTestSettings.Instance.TestDBConnectionString,
                DefaultBulkCopyBatchSize = AppTestSettings.Instance.DefaultBulkCopyBatchSize
            });

            var nasdaqLogger = new Mock<ILogger<NasdaqParser>>();
            var exchangeClientLogger = new Mock<ILogger<ExchangeClient>>();

            // Setup dependencies
            ExchangeSyncRepo = new ExchangeSyncRepository(mockDbOptions.Object);

            string nasdaqDownloadUrl = (await ExchangeSyncRepo.GetSyncSettings()).Url;

            // Stub FTP so we use downloaded file from disk instead of every test run
            var stubFTPClient = new CachedNasdaqClient(nasdaqDownloadUrl);

            var exchangeClient = new ExchangeClient(
                            ftpClient: stubFTPClient,
                            parser: new NasdaqParser(nasdaqLogger.Object),
                            logger: exchangeClientLogger.Object);

            var changesetFactory = new ExchangeSyncChangesetsFactory();

            ExchangeSync = new ExchangeSync(exchangeClient, ExchangeSyncRepo, changesetFactory, new Mock<ILogger<ExchangeSync>>().Object);
            SymbolRepo = new SymbolRepository(mockDbOptions.Object);

            await ToggleSyncThreshold(false);
            await ExchangeSync.Sync();
            await ToggleSyncThreshold(true);
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        private async Task ToggleSyncThreshold(bool enable)
        {
            var sql = @"UPDATE ExchangeSyncSetting SET DisableThresholds = @disable";
            using var connection = new SqlConnection(_connectionString);
            await connection.ExecuteAsync(sql, new { disable = !enable });
        }
    }
}
