using Dapper;
using FomoAPI.Application.DTOs;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ViewModels.Member
{
    /// <inheritdoc cref="IMemberQueries"/>
    public class MemberQueries : IMemberQueries
    {
        private readonly string _connectionString;

        public MemberQueries(IOptionsMonitor<DbOptions> dbOptions)
        {
            _connectionString = dbOptions.CurrentValue.ConnectionString;
        }

        public async Task<MembersViewModel> GetPaginatedMembers(int limit, int offset)
        {
            string sql = @"
                           SELECT Count(*) FROM AspNetUsers;

                           SELECT 
                                Id,
                                UserName Name
                           FROM
                                AspNetUsers
                           ORDER BY
                                UserName
                           OFFSET @offset ROWS
                           FETCH NEXT @limit ROWS ONLY";

            IEnumerable<MemberDTO> members;
            int total = 0;

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            using var multi = await connection.QueryMultipleAsync(sql, new { limit, offset });
            total = (await multi.ReadAsync<int>()).Single();
            members = (await multi.ReadAsync<MemberDTO>());
   

            return new MembersViewModel(members, total: total, limit: limit, offset: offset);
        }
    }
}
