using Dapper;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.LeaderBoard
{
    /// <summary>
    /// SQL query to populate the view model.
    /// </summary>
    public class LeaderBoardQueries : ILeaderBoardQueries
    {
        private string _connectionString;

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
            var boardValues = await GetStockRatingBoard(top, SortDirection.Descending);
            var board = new Board(BoardColumns.MostBullish, boardValues);
            return board;
        }

        private async Task<Board> GetMostBearish(int top)
        {
            var boardValues = await GetStockRatingBoard(top, SortDirection.Ascending);
            var board = new Board(BoardColumns.MostBearish, boardValues);
            return board;
        }

        private async Task<IEnumerable<BoardValue>> GetStockRatingBoard(int top, SortDirection sortDirection)
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
                return boardValues;
            }
        }

        private async Task<Board> GetBestPerformer(int top)
        {
            var boardValues = await GetUserRating(top, SortDirection.Descending);
            var board = new Board(BoardColumns.BestPerformer, boardValues);
            return board;
        }

        private async Task<Board> GetWorstPerformer(int top)
        {
            var boardValues = await GetUserRating(top, SortDirection.Ascending);
            var board = new Board(BoardColumns.WorstPerformer, boardValues);
            return board;
        }

        private async Task<IEnumerable<BoardValue>> GetUserRating(int top, SortDirection sortDirection)
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
                return boardValues;
            }
        }

        private string SetSortOrder(string sql, SortDirection sortDirection)
        {
            return sql.Replace("@SortDirection",  sortDirection == SortDirection.Ascending ? "ASC" : "DESC");
        }
    }
}
