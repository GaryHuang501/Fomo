using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Dapper;
using System.Collections.Generic;
using FomoAPI.Infrastructure.Enums;
using System.Linq;
using System;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <inheritdoc cref="IVoteRepository"></inheritdoc>/>
    public class VoteRepository : IVoteRepository
    {
        private readonly string _connectionString;

        public VoteRepository(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        public async Task SaveVote(Vote vote)
        {
            var sql = @"IF EXISTS( SELECT 1 FROM Vote WHERE SymbolId = @SymbolId AND UserId = @UserId)
                            BEGIN
                                UPDATE
                                    Vote
                                SET
                                    Direction = @Direction
                                 WHERE 
                                    SymbolId = @SymbolId
                                    AND 
                                    UserId = @UserId
                            END
                        ELSE
                            BEGIN
                                INSERT INTO Vote
                                (SymbolId, UserId, Direction, LastUpdated)
                                VALUES
                                (@SymbolId, @UserId, @Direction, @LastUpdated);
                            END;";

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.ExecuteAsync(sql, vote);
            }
        }

        public async Task<IReadOnlyDictionary<int, TotalVotes>> GetTotalVotes(ISet<int> symbolIds, Guid userId)
        {
            var sql = @"SELECT
                            TotalVote.SymbolId,
                            @UserId UserId,
                            TotalVote.Count,
                            COALESCE(MyVote.Direction, 0) [MyVoteDirection]
                        FROM
                            (
                            SELECT
                                SymbolId,
                                @UserId UserId,
                                SUM(Direction) [Count]
                            FROM
                                Vote
                            INNER JOIN
                                @TvpSymbolIds TvpSymbolIds           
                            ON
                                TvpSymbolIds.Id = Vote.SymbolId
                            GROUP BY
                                Vote.SymbolId
                            ) TotalVote
                        LEFT JOIN
                            Vote MyVote
                        ON
                            MyVote.UserId = @UserId
                            AND
                            MyVote.SymbolId = TotalVote.SymbolId;";
                             
            var symbolIdsTvp = symbolIds.ToDataTable(new ColumnSchema<int>("Id", typeof(int), id => id));

            using (var connection = new SqlConnection(_connectionString))
            {
                var args = new
                {
                    UserId = userId,
                    TvpSymbolIds = symbolIdsTvp.AsTableValuedParameter(TableType.IntIdType)
                };

                var votes = await connection.QueryAsync<TotalVotes>(sql, args);

                return votes.ToDictionary(v => v.SymbolId, v => v);
            }
        }
    }
}
