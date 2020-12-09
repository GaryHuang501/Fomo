using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for CRUD operations on a user's portfolio.
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
            var sql = @"INSERT INTO Portfolio (UserId, Name, DateCreated, DateModified)
                        OUTPUT Inserted.Id, Inserted.UserId, Inserted.Name, Inserted.DateModified, Inserted.DateCreated
                        VALUES
                        (@userId, @name, GETUTCDATE(), GETUTCDATE());";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QuerySingleAsync<Portfolio>(sql, new { userId, name });
            }
        }

        /// <summary>
        /// Add symbol to portfolio
        /// </summary>
        /// <param name="portfolioId">Id of Portfolio to add symbols to</param>
        /// <param name="symbolId">Id of symbol to add</param>
        /// <returns>Newly Added PortfolioSymbol. Return null if symbol was not added: either symbol Id not found or symbol already exists.</returns>
        public async Task<PortfolioSymbol> AddPortfolioSymbol(int portfolioId, int symbolId)
        {
            var sql = @"DECLARE @InsertedId TABLE 
                        (
                            Id INT NOT NULL
                        );

                        INSERT INTO PortfolioSymbol (PortfolioId, SymbolID, SortOrder)      
                        OUTPUT INSERTED.Id INTO @InsertedId(Id)          
                        SELECT TOP 1
                            @portfolioId,
                            Symbol.Id,
                            COALESCE((SELECT MAX(SortOrder) + 1 FROM PortfolioSymbol WHERE PortfolioId = @portfolioId), 1) SortOrder
                        FROM 
                            Symbol
                        WHERE
                            Symbol.Id = @symbolId
                            AND
	                        NOT EXISTS(
		                        SELECT 1 FROM PortfolioSymbol WHERE PortfolioId = @portfolioId AND SymbolId = Symbol.Id
	                        );

                        SELECT 
                            PortfolioSymbol.Id,
                            PortfolioSymbol.SymbolId,
                            Symbol.Ticker,
                            Symbol.ExchangeId,
                            Exchange.Name [ExchangeName],
                            Symbol.FullName,
                            Symbol.Delisted,
                            PortfolioSymbol.SortOrder
                        FROM 
                            PortfolioSymbol
                        INNER JOIN 
                            Symbol
                        ON
                            Symbol.Id = PortfolioSymbol.SymbolId
                        INNER JOIN
                            Exchange
                        ON
                            Exchange.Id = Symbol.ExchangeId
                        INNER JOIN
                            @InsertedId InsertedId
                        ON
                            InsertedId.Id = PortfolioSymbol.Id;
                        ";

            using (var connection = new SqlConnection(_connectionString))
            {
                var portfolioSymbol = await connection.QueryAsync<PortfolioSymbol>(sql, new { portfolioId, symbolId});
                return portfolioSymbol.SingleOrDefault();
            }
        }

        /// <summary>
        /// Delete symbol from portfolio
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to delete symbol from</param>
        /// <param name="symbolId">Id of symbol to delete</param>
        /// <returns>Task</returns>
        public async Task DeletePortfolioSymbol(int portfolioSymbolID)
        {
            var sql = @"DELETE PortfolioSymbol 
                        WHERE 
                            Id = @portfolioSymbolID";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, new { portfolioSymbolID });
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
                        INNER JOIN 
                            PortfolioSymbol
                        ON  
                            PortfolioSymbol.Id = PriceAlert.PortfolioSymbolId
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
                    try
                    {
                        await connection.ExecuteAsync(sql, new { portfolioId }, transaction: transaction);
                        await transaction.CommitAsync();
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
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

        /// <summary>
        /// Reorder the given portfolio symbol and update  ort order of other PortfolioSymbol in portfolio
        /// to match new ordering.
        /// </summary>
        /// <param name="portfolioId">Id of portfolio</param>
        /// <param name="portfolioSymbolId">Id of reordered portfolioSymbol</param>
        /// <param name="newSortOrder">New sort order</param>
        /// <returns>True if rows updated. Otherwise portfolio symbol was not found.</returns>
        public async Task<bool> ReorderPortfolioSymbol(int portfolioId, IDictionary<string, int> portfolioSymbolIdToSortOrder)
        {
            var reorderSQL = @"	UPDATE PortfolioSymbol
	                            SET	
		                            SortOrder = tvpNewSortOrder.SortOrder
	                            FROM 
		                            PortfolioSymbol
	                            INNER JOIN
		                            @tvpNewSortOrder tvpNewSortOrder
	                            ON
		                            PortfolioSymbol.Id = tvpNewSortOrder.PortfolioSymbolId
                                WHERE
                                    PortfolioSymbol.PortfolioId = @portfolioId";


            var reorderTableValueData = portfolioSymbolIdToSortOrder.ToDataTable("PortfolioSymbolId", "SortOrder");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = await connection.BeginTransactionAsync())
                {
                    try
                    {
                        var rowsUpdated = await connection.ExecuteAsync(reorderSQL,
                            new
                            {
                                portfolioId,
                                tvpNewSortOrder = reorderTableValueData.AsTableValuedParameter(TableType.PortfolioSymbolSortOrderType)
                            },
                            transaction
                        );

                        await transaction.CommitAsync();

                        return rowsUpdated > 0;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
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
                                        PortfolioSymbol.Id,
                                        PortfolioSymbol.SymbolId,
                                        Symbol.Ticker,
                                        Symbol.FullName,
                                        Symbol.ExchangeId,
                                        Symbol.Delisted,
                                        Exchange.Name [ExchangeName],                        
                                        PortfolioSymbol.SortOrder
                                    FROM
                                        PortfolioSymbol
                                    INNER JOIN
                                        Symbol
                                    ON
                                        Symbol.Id = PortfolioSymbol.SymbolId
                                    INNER JOIN
                                        Exchange
                                    ON
                                        Exchange.Id = Symbol.ExchangeId
                                    WHERE
                                        PortfolioSymbol.PortfolioId = @portfolioId";

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var multiCommand = await connection.QueryMultipleAsync(getPortfolioSql, new { portfolioId }))
                {
                    var portfolioResultSet = await multiCommand.ReadAsync<Portfolio>();
                    var portfolioSymbolsResultSet = await multiCommand.ReadAsync<PortfolioSymbol>();

                    portfolio = portfolioResultSet.FirstOrDefault();

                    if(portfolio != null)
                    {
                        portfolio = new Portfolio(
                            id: portfolio.Id,
                            userId: portfolio.UserId,
                            name: portfolio.Name,
                            dateModified: portfolio.DateModified,
                            dateCreated: portfolio.DateCreated,
                            symbols: portfolioSymbolsResultSet);
                    }
                }
            }

            return portfolio;
        }

        /// <summary>
        /// Get the ids of the portfolios the user owns.
        /// </summary>
        /// <param name="userId">User Guid ID</param>
        /// <returns>IEnumerable<int> containing the portfolio ids.</returns>
        public async Task<IEnumerable<int>> GetPortfolioIds(Guid userId)
        {
            var sql = @"SELECT 
                        Portfolio.Id 
                    FROM 
                        Portfolio
                    WHERE 
                        Portfolio.UserId = @UserId";

            using (var connection = new SqlConnection(_connectionString))
            {
                var ids = await connection.QueryAsync<int>(sql, new { userId });
                return ids;
            }
        }
    }
}
