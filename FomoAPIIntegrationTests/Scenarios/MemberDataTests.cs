using FomoAPI.Application.ViewModels;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPIIntegrationTests.Fixtures;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    public class MemberDataTests : IClassFixture<FomoApiFixture>
    {
        private readonly HttpClient _client;

        public MemberDataTests(FomoApiFixture webApiFactoryFixture)
        {
            webApiFactoryFixture.CreateServer(FomoApiFixture.WithNoHostedServices);
            _client = webApiFactoryFixture.GetClientNotAuth();

            var mockDbOptions = new Mock<IOptionsMonitor<DbOptions>>();
            mockDbOptions.Setup(x => x.CurrentValue).Returns(new DbOptions
            {
                ConnectionString = AppTestSettings.Instance.TestDBConnectionString
            });

        }

        [Fact]
        public async Task Should_ReturnMembersGroupedByTheirFirstLetterAlphabetically()
        {
            var a_users = new List<Guid>();
            var d_users = new List<Guid>();
            var test_user = "60811430-C2CF-491F-DD9F-08D827B943FE"; // default test user;
            var z_users = new List<Guid>();
            var other_users = new List<Guid>();

            try
            {
                a_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Abe"));

                d_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Dave"));
                d_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Dan"));

                z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zack"));
                z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zane"));
                z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zoro"));
                z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zax"));

                other_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "!12AK47"));
                other_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "__UK__"));

                var membersResponse = await _client.GetAsync(ApiPath.MembersData(100, 0));

                var viewModel = await membersResponse.Content.ReadAsAsync<MembersViewModel>();

                int total = a_users.Count + d_users.Count + z_users.Count + other_users.Count + 1;

                Assert.Equal(total, viewModel.Total);

                Assert.Equal(a_users.Count, viewModel.MemberGroupings['A'].Count);
                Assert.Equal(d_users.Count, viewModel.MemberGroupings['D'].Count);
                Assert.Single(viewModel.MemberGroupings['F']);
                Assert.Equal(z_users.Count, viewModel.MemberGroupings['Z'].Count);
                Assert.Equal(other_users.Count, viewModel.UncategorizedMembers.Count());

                Assert.True((new string[]{ "Abe" }).SequenceEqual(viewModel.MemberGroupings['A'].Select(m => m.Name)));
                Assert.True((new string[] { "Dan", "Dave" }).SequenceEqual(viewModel.MemberGroupings['D'].Select(m => m.Name)));
                Assert.True((new string[] { "Zack", "Zane", "Zax", "Zoro" }).SequenceEqual(viewModel.MemberGroupings['Z'].Select(m => m.Name)));
                Assert.True((new string[] { "!12AK47", "__UK__" }).SequenceEqual(viewModel.UncategorizedMembers.Select(m => m.Name)));

                Assert.Equal(test_user.ToLower(), viewModel.MemberGroupings['F'].Single().Id.ToString().ToLower());
            }
            finally
            {
                await TestUtil.ClearUsers(AppTestSettings.Instance.TestDBConnectionString, a_users);
                await TestUtil.ClearUsers(AppTestSettings.Instance.TestDBConnectionString, d_users);
                await TestUtil.ClearUsers(AppTestSettings.Instance.TestDBConnectionString, z_users);
                await TestUtil.ClearUsers(AppTestSettings.Instance.TestDBConnectionString, other_users);
            }
        }
    }
}
