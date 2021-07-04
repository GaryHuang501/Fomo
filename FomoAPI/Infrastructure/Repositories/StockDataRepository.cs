using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Stocks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <inheritdoc cref="IStockDataRepository"></inheritdoc>/>
    public class StockDataRepository : IStockDataRepository
    {
        private string _connectionString;
        public StockDataRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        /// <summary>
        /// Get Single quote data from database.
        /// </summary>
        /// <param name="symbolId">SymbolId of stock data to retrieve.</param>
        /// <returns><see cref="SingleQuoteData"/></returns>
        /// <remarks>
        /// Is not thread safe if same items are inserted at the same time. 
        /// But that's fine since the stock data is unlikely to be different if it's that close in time.
        /// This is more performant than adding locks.
        /// </remarks>
        public async Task<SingleQuoteData> GetSingleQuoteData(int symbolId)
        {
            var sql = @"SELECT
                            SymbolId,
                            Symbol.Ticker,
                            Price,
                            Change,
                            ChangePercent,
                            LastUpdated
                        FROM
                            SingleQuoteData
                        INNER JOIN
                            Symbol
                        ON
                            Symbol.Id = SingleQuoteData.SymbolId
                        WHERE
                            SymbolId = @SymbolId";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleOrDefaultAsync<SingleQuoteData>(sql, new { SymbolId = symbolId });
        }

        public async Task<bool> UpsertSingleQuoteData(UpsertSingleQuoteData data)
        {
            var sql = @"IF EXISTS( SELECT 1 FROM SingleQuoteData WHERE SymbolId = @SymbolId)
                            BEGIN
                                UPDATE
                                    SingleQuoteData
                                SET
                                    Price = @Price,
                                    Change = @Change,
                                    ChangePercent = @ChangePercent,
                                    LastUpdated = @LastUpdated
                                WHERE
                                    SymbolId = @SymbolId;
                            END
                        ELSE
                            BEGIN
                                INSERT INTO SingleQuoteData
                                (SymbolId, Price, Change, ChangePercent,LastUpdated)
                                VALUES
                                (@SymbolId, @Price, @Change, @ChangePercent, @LastUpdated);
                            END;";

            using (var connection = new SqlConnection(_connectionString))
            {
                int affectedRows =  await connection.ExecuteAsync(sql, data);

                return affectedRows == 1;
            }        
        }
    }
}
