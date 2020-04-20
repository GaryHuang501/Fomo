﻿using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
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
        public PortfolioRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        /// <summary>
        /// Create new portfolio
        /// </summary>
        /// <param name="userId">UserId for owner of portfolio</param>
        /// <param name="name">Portfolio name</param>
        /// <returns></returns>
        public async Task<Portfolio> CreatePortfolio(Guid userId, string name)
        {
            var sql = @"INSERT INTO Portfolio (UserId, Name, DateCreated)
                        OUTPUT Inserted.Id, Inserted.UserId, Inserted.Name, Inserted.DateCreated
                        VALUES
                        (@userId, @name, GETUTCDATE());";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleAsync<Portfolio>(sql, new { userId, name });
            }
        }

        /// <summary>
        /// Add symbol to portfolio
        /// </summary>
        /// <param name="portfolioId">Id of Portfolio to add symbols to</param>
        /// <param name="symbolId"></param>
        /// <returns>Successfully added row or not.</returns>
        public async Task<bool> AddPortfolioSymbol(int portfolioId, string ticker, string exchange)
        {
            var sql = @"INSERT INTO PortfolioSymbol (PortfolioId, SymbolID, SortOrder)
                        SELECT TOP 1
                            @portfolioId,
                            Symbol.Id,
                            COALESCE((SELECT MAX(SortOrder) FROM PortfolioSymbol WHERE PortfolioId = @portfolioId), 1)
                        FROM Symbol
                        WHERE
                            Ticker = @ticker
                            AND ExchangeName = @exchange";

            using (var connection = new SqlConnection(_connectionString))
            {
                var rowsAffected = await connection.ExecuteAsync(sql, new { portfolioId, ticker, exchange });

                return rowsAffected > 0;
            }
        }

        /// <summary>
        /// Remove symbol from portfolio
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to remove symbol from</param>
        /// <param name="symbolId">Id of symbol to remove</param>
        /// <returns>Task</returns>
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

        /// <summary>
        /// Delete portfolio
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to delete</param>
        /// <returns>Task</returns>
        public async Task DeletePortfolio(int portfolioId)
        {
            var sql = @"DELETE PriceAlert FROM PriceAlert
                        INNER JOIN PortfolioSymbol
                        ON  PortfolioSymbol.Id = PriceAlert.PortfolioSymbolId
                        WHERE
                            PortfolioSymbol.PortfolioId = @portfolioId;

                        DELETE FROM PortfolioSymbol
                        WHERE
                            PortfolioSymbol.PortfolioId = @portfolioId;
                         
                        DELETE Portfolio
                        WHERE 
                            Id = @portfolioId;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.ExecuteAsync(sql, new { portfolioId }, transaction: transaction);
                    await transaction.CommitAsync();
                }
            }

        }

        /// <summary>
        /// Rename portfolio to given name
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to rename</param>
        /// <param name="newName">New name for the portfolio</param>
        /// <returns>If portfolio was successfully updated</returns>
        public async Task<bool> RenamePortfolio(int portfolioId, string newName)
        {
            var sql = @"UPDATE Portfolio
                        SET
                            Name = @newName
                        WHERE 
                            Id = @portfolioId;";

            using (var connection = new SqlConnection(_connectionString))
            {
                var rowsUpdated = await connection.ExecuteAsync(sql, new { portfolioId, newName });

                return rowsUpdated > 0;
            }
        }

        public async Task AddPriceAlert(Guid userId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the portfolio and it's symbols
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to fetch</param>
        /// <returns>Portfolio. Null if does not exist.</returns>
        public async Task<Portfolio> GetPortfolio(int portfolioId)
        {
            Portfolio portfolio;

            var getPortfolioSql = @"SELECT
                                        Id,
                                        UserId,
                                        Name,
                                        DateCreated
                                    FROM
                                        Portfolio
                                    WHERE
                                        Id = @portfolioId

                                    SELECT
                                        Symbol.Id,
                                        Symbol.Ticker,
                                        Symbol.FullName,
                                        Symbol.ExchangeName
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

                    portfolio = portfolioResultSet.FirstOrDefault();

                    if(portfolio != null)
                    {
                        portfolio.Symbols = portfolioSymbolsResultSet;
                    }
                }
            }

            return portfolio;
        }
    }
}
