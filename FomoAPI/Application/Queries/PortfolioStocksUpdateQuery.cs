using Dapper;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Queries
{
    public class PortfolioStocksUpdateQuery : IPortfolioStocksUpdateQuery
    {
        private string _connectionString;

        public PortfolioStocksUpdateQuery(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        public async Task<IEnumerable<int>> Get(int top, DateTime maxDate)
        {
            string sql = @" SELECT DISTINCT TOP(@top)
                                PortfolioSymbol.SymbolID,
                                SingleQuoteData.LastUpdated
                            FROM
	                            Portfolio
                            INNER JOIN
                                PortfolioSymbol
                            ON
                                Portfolio.Id = PortfolioSymbol.PortfolioId
                            INNER JOIN
                                SingleQuoteData 
                            ON
                                SingleQuoteData.SymbolId = PortfolioSymbol.SymbolId
                            WHERE
                                SingleQuoteData.LastUpdated <= @maxDate
                            ORDER BY SingleQuoteData.LastUpdated;";

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync<int>(sql, new { top, maxDate });
            }
        }

    }
}
