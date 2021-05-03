using FomoAPI.Application.ViewModels;
using FomoAPI.Application.ViewModels.Member;
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
    public class MemberDataTests : IClassFixture<FomoApiFixture>, IAsyncLifetime
    {
        private readonly HttpClient _client;

        public MemberDataTests(FomoApiFixture webApiFactoryFixture)
        {
            webApiFactoryFixture.CreateServer(FomoApiFixture.WithNoHostedServices);
            _client = webApiFactoryFixture.GetClientNotAuth();
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            var cleanFixture = new CleanDBFixture();

            await cleanFixture.InitializeAsync();
        }

        [Fact]
        public async Task Should_ReturnMembersGroupedByTheirFirstLetterAlphabetically()
        {
            var a_users = new List<Guid>();
            var d_users = new List<Guid>();
            var z_users = new List<Guid>();
            var other_users = new List<Guid>();

            a_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Abe"));

            d_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Dave"));
            d_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Dan"));

            z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zack"));
            z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zane"));
            z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zoro"));
            z_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "Zax"));

            other_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "!12AK47"));
            other_users.Add(await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, "__UK__"));

            var httpMessage = new HttpRequestMessage(HttpMethod.Get, ApiPath.MembersData(100, 0));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, a_users.First().ToString());

            var membersResponse = await _client.SendAsync(httpMessage);

            var viewModel = await membersResponse.Content.ReadAsAsync<MembersViewModel>();

            int total = a_users.Count + d_users.Count + z_users.Count + other_users.Count;

            Assert.Equal(total, viewModel.Total);

            Assert.Equal(a_users.Count, viewModel.MemberGroupings['A'].Count);
            Assert.Equal(d_users.Count, viewModel.MemberGroupings['D'].Count);
            Assert.Equal(z_users.Count, viewModel.MemberGroupings['Z'].Count);
            Assert.Equal(other_users.Count, viewModel.UncategorizedMembers.Count());

            Assert.True((new string[] { "Abe" }).SequenceEqual(viewModel.MemberGroupings['A'].Select(m => m.Name)));
            Assert.True((new string[] { "Dan", "Dave" }).SequenceEqual(viewModel.MemberGroupings['D'].Select(m => m.Name)));
            Assert.True((new string[] { "Zack", "Zane", "Zax", "Zoro" }).SequenceEqual(viewModel.MemberGroupings['Z'].Select(m => m.Name)));
            Assert.True((new string[] { "!12AK47", "__UK__" }).SequenceEqual(viewModel.UncategorizedMembers.Select(m => m.Name)));
        }
    }
}
