using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPIIntegrationTests
{
    public static class TestUtil
    {
        public static async Task<Guid> CreateNewUser(string dbConnectionString, string userName = null, string userEmail = null)
        {
            var newUserId = Guid.NewGuid();

            var sql = @"INSERT INTO [dbo].[AspNetUsers]
                           ([Id]
                           ,[UserName]
                           ,[NormalizedUserName]
                           ,[Email]
                           ,[NormalizedEmail]
                           ,[EmailConfirmed]
                           ,[PasswordHash]
                           ,[SecurityStamp]
                           ,[ConcurrencyStamp]
                           ,[PhoneNumber]
                           ,[PhoneNumberConfirmed]
                           ,[TwoFactorEnabled]
                           ,[LockoutEnd]
                           ,[LockoutEnabled]
                           ,[AccessFailedCount])
                     VALUES
                           (
			                @UserId,
			                @UserName,  
                            @UserName,
			                @UserEmail,
			                @UserEmail,
			                1,
			                null,
			                null,
			                null,
			                null,
			                0,
			                0,
			                null,
			                0,
			                0)";

            using var connection = new SqlConnection(dbConnectionString);
            
            await connection.ExecuteAsync(sql, new { 
                UserId = newUserId, 
                UserEmail = $"{userEmail ?? newUserId.ToString()}@fomo-app.com",
                UserName = userName ?? newUserId.ToString().ToUpper()
            });

            return newUserId;
        }
    }
}
