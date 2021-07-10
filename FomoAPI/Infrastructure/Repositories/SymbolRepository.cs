using Dapper;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <inheritdoc cref="ISymbolRepository"></inheritdoc>/>
    public class SymbolRepository : ISymbolRepository
    {
        private readonly string _connectionString;

        public SymbolRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        public async Task<IEnumerable<Symbol>> GetSymbols(IEnumerable<string> tickers)
        {
            var sql = @"SELECT
                            Symbol.Id,
                            Symbol.Ticker,
                            Symbol.FullName,
                            Exchange.Id [ExchangeId],
                            Exchange.Name [ExchangeName],
                            Symbol.Delisted
                        FROM
                            Symbol
                        INNER JOIN
                            Exchange
                        ON
                            Exchange.Id = Symbol.ExchangeId
                        WHERE
                            Ticker IN @Tickers";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Symbol>(sql, new { Tickers = tickers });
        }

        public async Task<IEnumerable<Symbol>> GetSymbols(IEnumerable<int> symbolIds)
        {
            var sql = @"SELECT
                            Symbol.Id,
                            Symbol.Ticker,
                            Symbol.FullName,
                            Exchange.Id [ExchangeId],
                            Exchange.Name [ExchangeName],
                            Symbol.Delisted
                        FROM
                            Symbol
                        INNER JOIN
                            Exchange
                        ON
                            Exchange.Id = Symbol.ExchangeId
                        WHERE
                            Symbol.Id IN @SymbolIds";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Symbol>(sql, new { SymbolIds = symbolIds });
        }

        public async Task<Symbol> GetSymbol(string ticker)
        {
            IEnumerable<Symbol> result = await GetSymbols(new string[] { ticker });

            return result.FirstOrDefault();
        }


        public async Task<Symbol> GetSymbol(int symbolId)
        {
            IEnumerable<Symbol> result = await GetSymbols(new int[] { symbolId });

            return result.FirstOrDefault();
        }
    }
}
