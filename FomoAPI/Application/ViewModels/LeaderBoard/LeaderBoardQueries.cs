using Dapper;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    public class LeaderBoardQueries : ILeaderBoardQueries
    {
        private string _connectionString;

        private const string MostBullishHeader = "Most Bullish Stock";

        private const string MostBearishHeader = "Most Bearish Stock";

        private const string BestPerformingHeader = "Best Performing User";

        private const string WorstPerformerHeader = "Worst Performing User";

        public LeaderBoardQueries(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        public async Task<LeaderBoardViewModel> GetLeaderBoardData(int top)
        {
            var leaderBoard = new LeaderBoardViewModel(
                    mostBullish: await GetMostBullish(top),
                    mostBearish: await GetMostBearish(top),
                    bestPerformers: await GetBestPerformer(top),
                    worstPerformers: await GetWorstPerformer(top)
                );

            return leaderBoard;
        }

        private async Task<Board> GetMostBullish(int top)
        {
            return await GetStockRatingBoard(top, MostBullishHeader, SortDirection.Descending);
        }

        private async Task<Board> GetMostBearish(int top)
        {
            return await GetStockRatingBoard(top, MostBearishHeader, SortDirection.Ascending);
        }

        private async Task<Board> GetStockRatingBoard(int top, string header, SortDirection sortDirection)
        {
            string sql = @" SELECT
                                CAST(Id AS VARCHAR(36)) [Id],
                                Name,
                                CAST(Value AS VARCHAR(20)) [Value]
                            FROM
                            (
                                SELECT TOP(@Top)
                                    Symbol.Id [Id],
                                    MAX(Symbol.Ticker) [Name],
                                    SUM(Vote.Direction) [Value]
                                FROM
                                    Vote
                                INNER JOIN
                                    Symbol
                                ON
                                    Vote.SymbolId = Symbol.Id
                                GROUP BY
                                    Symbol.Id
                                ORDER BY Value @SortDirection
                            ) Result";

            sql = SetSortOrder(sql, sortDirection);

            using (var connection = new SqlConnection(_connectionString))
            {
                var boardValues = await connection.QueryAsync<BoardValue>(sql, new { Top = top });
                return new Board(header, boardValues);
            }
        }

        private async Task<Board> GetBestPerformer(int top)
        {
            return await GetUserRating(top, BestPerformingHeader, SortDirection.Descending);
        }

        private async Task<Board> GetWorstPerformer(int top)
        {
            return await GetUserRating(top, WorstPerformerHeader, SortDirection.Ascending);
        }

        private async Task<Board> GetUserRating(int top, string header, SortDirection sortDirection)
        {
            string sql = @"
                            SELECT
                                CAST(Id AS VARCHAR(36)) [Id],
                                Name,
                                CAST((FORMAT([AveragePrice], 'N2')) AS VARCHAR(20)) [Value]
                            FROM
                            (
                                SELECT TOP(@top)
                                    AspNetUser.Id [Id],
                                    MAX(AspNetUser.UserName) [Name],
                                    SUM((SingleQuoteData.Price - PortfolioSymbol.AveragePrice) / NULLIF(PortfolioSymbol.AveragePrice, 0)) / COUNT(AspNetUser.Id ) * 100 [AveragePrice]
                                FROM
                                    AspNetUsers AspNetUser
                                INNER JOIN
	                                Portfolio
                                ON
	                                Portfolio.UserId = AspNetUser.Id
                                INNER JOIN
                                    PortfolioSymbol
                                ON
                                    Portfolio.Id = PortfolioSymbol.PortfolioId
                                INNER JOIN
                                    SingleQuoteData 
                                ON
                                    SingleQuoteData.SymbolId = PortfolioSymbol.SymbolId
                                WHERE
								    AveragePrice IS NOT NULL AND AveragePrice > 0
                                GROUP BY
                                    AspNetUser.Id
                                ORDER BY AveragePrice @SortDirection
                             ) Result";

            sql = SetSortOrder(sql, sortDirection);

            using (var connection = new SqlConnection(_connectionString))
            {
                var boardValues = await connection.QueryAsync<BoardValue>(sql, new { Top = top });
                return new Board(header, boardValues);
            }
        }

        private string SetSortOrder(string sql, SortDirection sortDirection)
        {
            return sql.Replace("@SortDirection",  sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
        }
    }
}
