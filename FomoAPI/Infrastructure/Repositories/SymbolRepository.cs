using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for CRUD operations on the stock symbol/ticker info and data.
    /// </summary>
    public class SymbolRepository : ISymbolRepository
    {
        private string _connectionString;
        public SymbolRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        /// <summary>
        /// Get symbols matching the keyword. 
        /// Will return symbols wherhe ticker start with the keyword or full name contains key word.
        /// </summary>
        /// <param name="keyword">keyword to search for</param>
        /// <returns>IEnumerable of Symbols</returns>
        public async Task<IEnumerable<Symbol>> GetSymbols(string keyword)
        {
            var sql = @"SELECT TOP 5
                            Id,
                            Ticker,
                            FullName,
                            ExchangeName
                        FROM
                            Symbol
                        WHERE
                            Ticker LIKE @tickerSearch
                            OR
                            FullName LIKE @fullNameSearch;";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<Symbol>(sql, new { tickerSearch = $"{keyword}%", fullNameSearch = $"%{keyword}%" });
            }
        }
    }
}
