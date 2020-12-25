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
    public class CleanDBFixture : DBFixture
    {
        public CleanDBFixture()
        {
        }

        public override async Task InitializeAsync()
        {
            var deleteSingleQuoteDataSql = @"DELETE FROM SingleQuoteData;";
            await Connection.ExecuteAsync(deleteSingleQuoteDataSql, null);

            var deletePortfolioSymbolSql = @"DELETE FROM PortfolioSymbol;";
            await Connection.ExecuteAsync(deletePortfolioSymbolSql, null);

            var deletePortfolioSql = @"DELETE FROM Portfolio;";
            await Connection.ExecuteAsync(deletePortfolioSql, null);

            var deleteSymbolSql = @"DELETE FROM Symbol";
            await Connection.ExecuteAsync(deleteSymbolSql, null);

            var deleteExchangeSyncHistory = @"DELETE FROM ExchangeSyncHistory";
            await Connection.ExecuteAsync(deleteExchangeSyncHistory, null);
        }
    }
}
