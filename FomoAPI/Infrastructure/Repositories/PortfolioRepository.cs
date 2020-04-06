using Dapper;
using FomoAPI.Domain.Stocks;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository class for CRUD operations on a user's portfolio.
    /// Security layer should be responsible for restricting 
    /// portfolio access to the user.
    /// </summary>
    public class PortfolioRepository : IPortfolioRepository
    {
        private string _connectionString;
        public PortfolioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Portfolio> CreatePortfolio(Guid userId, string name)
        {
            var sql = @"INSERT INTO Portfolio (UserId, Name, DateCreated, DateModified)
                        OUTPUT Inserted.UserId, Inserted.Name, Inserted.DateCreated, Inserted.DateModified
                        VALUES
                        (@userId, @name, GETDATE(), GETDATE());";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleAsync<Portfolio>(sql, new { userId, name });
            }
        }

        public async Task AddPortfolioSymbol(int portfolioId, int symbolId)
        {
            var sql = @"INSERT INTO PortfolioSymbol (PortfolioId, SymbolID)
                        (@portfolioID, @symbolID);";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, new { portfolioId, symbolId });
            }
        }

        public async Task RemovePortfolioSymbol(int portfolioId, int symbolId)
        {
            var sql = @"DELETE PortfolioSymbol 
                        WHERE 
                            PortfolioId = @portfolioId
                            AND
                            SymbolId = @symbolId";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, new { portfolioId, symbolId });
            }
        }

        public async Task DeletePortfolio(int portfolioId)
        {
            var sql = @"DELETE Portfolio
                        WHERE 
                            Id = @portfolioId;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, new { portfolioId });
            }
        }

        public async Task AddPriceAlert(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Portfolio> GetPortfolio(int portfolioId)
        {
            Portfolio portfolio;

            var getPortfolioSql = @"SELECT
                                        UserId,
                                        Name,
                                        DateCreated,
                                        DateLastModified
                                    FROM
                                        Portfolio
                                    WHERE
                                        Id = @portfolioId

                                    SELECT
                                        Id,
                                        Name,
                                        ExchangeName
                                    FROM
                                        PortfolioSymbol
                                    INNER JOIN
                                        Symbol
                                    ON
                                        Symbol.Id = PortfolioSymbol.SymbolId
                                    WHERE
                                        PortfolioSymbol.PortfolioId = @portfolioId";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multiCommand = await connection.QueryMultipleAsync(getPortfolioSql, new { portfolioId }))
                {
                    var portfolioResultSet = await multiCommand.ReadAsync<Portfolio>();
                    var portfolioSymbolsResultSet = await multiCommand.ReadAsync<Symbol>();

                    portfolio = portfolioResultSet.First();
                    portfolio.Symbols = portfolioSymbolsResultSet;
                }
            }

            return portfolio;
        }
    }
}
