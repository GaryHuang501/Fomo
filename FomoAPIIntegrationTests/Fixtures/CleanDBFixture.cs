using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Fixtures
{
    /// <summary>
    /// Fixture to clear out non user data from database
    /// to create allow for clean slate.
    /// </summary>
    public class CleanDBFixture : IAsyncLifetime
    {
        private readonly SqlConnection _connection;

        public CleanDBFixture()
        {
            _connection = new SqlConnection(AppTestSettings.Instance.TestDBConnectionString);
        }

        public async Task InitializeAsync()
        {
            var deletePortfolioSymbolSql = @"DELETE FROM PortfolioSymbol;";
            await _connection.ExecuteAsync(deletePortfolioSymbolSql, null);

            var deletePortfolioSql = @"DELETE FROM Portfolio;";
            await _connection.ExecuteAsync(deletePortfolioSql, null);

            var deleteSymbolSql = @"DELETE FROM Symbol";
            await _connection.ExecuteAsync(deleteSymbolSql, null);

            var deleteExchangeSyncHistory = @"DELETE FROM ExchangeSyncHistory";
            await _connection.ExecuteAsync(deleteExchangeSyncHistory, null);
        }

        public async Task DisposeAsync()
        {
            await _connection.DisposeAsync();
        }
    }
}
