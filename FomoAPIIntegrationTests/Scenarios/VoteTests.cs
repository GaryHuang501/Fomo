using Dapper;
using FomoAPI.Application.Commands.Vote;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Repositories;
using FomoAPIIntegrationTests.Fixtures;
using Microsoft.Data.SqlClient;
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
    public class VoteTests : IClassFixture<FomoApiFixture>, IClassFixture<ExchangeSyncSetupFixture>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly ISymbolRepository _symbolRepo;

        public VoteTests(FomoApiFixture webApiFactoryFixture)
        {
            webApiFactoryFixture.CreateServer(FomoApiFixture.WithNoHostedServices);
            _client = webApiFactoryFixture.GetClientNotAuth();

            var mockDbOptions = new Mock<IOptionsMonitor<DbOptions>>();
            mockDbOptions.Setup(x => x.CurrentValue).Returns(new DbOptions
            {
                ConnectionString = AppTestSettings.Instance.TestDBConnectionString
            });

            _symbolRepo = new SymbolRepository(mockDbOptions.Object);
        }

        public async Task InitializeAsync()
        {
            using (var connection = new SqlConnection(AppTestSettings.Instance.TestDBConnectionString))
            {
                await connection.ExecuteAsync("DELETE FROM Vote", null);
            }
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task Should_HandleGettingVotes_WhenNoVotes()
        {
            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");
            var fetchResponse = await _client.GetAsync($"{ApiPath.Votes}?sid={jpmSymbol.Id}");
            Dictionary<int, TotalVotes> voteData = await fetchResponse.Content.ReadAsAsync<Dictionary<int, TotalVotes>>();

            Assert.Empty(voteData);
        }

        [Fact]
        public async Task Should_GetVotesOnlyWhenSymbolMatches()
        {
            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");

            var voteCommand = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.UpVote };

            var saveResponse = await _client.PostAsync(ApiPath.Votes, voteCommand.ToJsonPayload());

            saveResponse.EnsureSuccessStatusCode();

            var fetchResponse = await _client.GetAsync($"{ApiPath.Votes}?sid={int.MaxValue}");
            Dictionary<int, TotalVotes> voteData = await fetchResponse.Content.ReadAsAsync<Dictionary<int, TotalVotes>>();

            Assert.Empty(voteData);
        }


        [Theory]
        [InlineData(VoteDirection.UpVote)]
        [InlineData(VoteDirection.DownVote)]
        [InlineData(VoteDirection.None)]
        public async Task Should_SaveVoteForUser(VoteDirection voteDirection)
        {
            string user1Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();

            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");

            var voteCommand = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = voteDirection };

            await SendVoteForCustomUser(voteCommand, user1Id);

            Dictionary<int, TotalVotes> voteData = await GetVoteData(user1Id, new int[] { jpmSymbol.Id});

            Assert.Single(voteData);
            Assert.True(voteData.ContainsKey(jpmSymbol.Id));

            var totalVotes = voteData[jpmSymbol.Id];
            Assert.Equal((int)voteDirection, totalVotes.Count);
            Assert.Equal(voteDirection, totalVotes.MyVoteDirection);
            Assert.Equal(jpmSymbol.Id, totalVotes.SymbolId);
            Assert.Equal(user1Id, totalVotes.UserId.ToString());
        }

        [Fact]
        public async Task Should_HandleUpdatingDownVoteToUpVote()
        {
            string user1Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();

            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");

            var downVoteCommand = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.DownVote };

            await SendVoteForCustomUser(downVoteCommand, user1Id);

            var upVoteCommand = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.UpVote };

            await SendVoteForCustomUser(upVoteCommand, user1Id);

            Dictionary<int, TotalVotes> voteData = await GetVoteData(user1Id, new int[] { jpmSymbol.Id });

            Assert.Single(voteData);
            Assert.True(voteData.ContainsKey(jpmSymbol.Id));

            var totalVotes = voteData[jpmSymbol.Id];
            Assert.Equal(1, totalVotes.Count);
            Assert.Equal(VoteDirection.UpVote, totalVotes.MyVoteDirection);
            Assert.Equal(user1Id, totalVotes.UserId.ToString());
        }

        [Fact]
        public async Task Should_StillReturnTotalVote_WhenUserHasNeverVoted()
        {
            string user1Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();
            string user2Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();

            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");

            var voteCommand = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.DownVote };

            await SendVoteForCustomUser(voteCommand, user1Id); 

            Dictionary<int, TotalVotes> voteData = await GetVoteData(user2Id, new int[] { jpmSymbol.Id });

            Assert.Single(voteData);
            Assert.True(voteData.ContainsKey(jpmSymbol.Id));

            var jpmVoteData = voteData[jpmSymbol.Id]; 

            Assert.Equal((int)VoteDirection.DownVote, jpmVoteData.Count);

            Assert.Equal(VoteDirection.None, jpmVoteData.MyVoteDirection);
        }

        [Fact]
        public async Task Should_SumVotesForMultipleUserAndSymbols()
        {
            string user1Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();
            string user2Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();
            string user3Id = (await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString)).ToString();

            Symbol jpmSymbol = await _symbolRepo.GetSymbol("JPM");
            Symbol bacSymbol = await _symbolRepo.GetSymbol("BAC");

            var voteCommandJpm1 = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.DownVote };
            var voteCommandJpm2 = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.None };
            var voteCommandJpm3 = new VoteCommand { SymbolId = jpmSymbol.Id, Direction = VoteDirection.UpVote };

            var voteCommandBac1 = new VoteCommand { SymbolId = bacSymbol.Id, Direction = VoteDirection.DownVote };
            var voteCommandBac2 = new VoteCommand { SymbolId = bacSymbol.Id, Direction = VoteDirection.DownVote };
            var voteCommandBac3 = new VoteCommand { SymbolId = bacSymbol.Id, Direction = VoteDirection.DownVote };

            await SendVoteForCustomUser(voteCommandJpm1, user1Id);
            await SendVoteForCustomUser(voteCommandJpm2, user2Id);
            await SendVoteForCustomUser(voteCommandJpm3, user3Id);

            await SendVoteForCustomUser(voteCommandBac1, user1Id);
            await SendVoteForCustomUser(voteCommandBac2, user2Id);
            await SendVoteForCustomUser(voteCommandBac3, user3Id);

            Dictionary<int, TotalVotes> voteData = await GetVoteData(user1Id, new int[] { jpmSymbol.Id, bacSymbol.Id });

            Assert.Equal(2, voteData.Count);
            Assert.True(voteData.ContainsKey(jpmSymbol.Id));
            Assert.True(voteData.ContainsKey(bacSymbol.Id));

            var jpmVoteData = voteData[jpmSymbol.Id];
            var bacVoteData = voteData[bacSymbol.Id];

            Assert.Equal(0, jpmVoteData.Count);
            Assert.Equal(-3, bacVoteData.Count);

            Assert.Equal(voteCommandJpm1.Direction, jpmVoteData.MyVoteDirection);
            Assert.Equal(voteCommandBac1.Direction, bacVoteData.MyVoteDirection);
        }

        private async Task<Dictionary<int, TotalVotes>> GetVoteData(string userId, int[] symbolIds)
        {
            var httpMessage = new HttpRequestMessage(HttpMethod.Get, ApiPath.GetVotes(symbolIds));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId);
            var fetchResponse = await _client.SendAsync(httpMessage);

            fetchResponse.EnsureSuccessStatusCode();

            Dictionary<int, TotalVotes> voteData = await fetchResponse.Content.ReadAsAsync<Dictionary<int, TotalVotes>>();

            return voteData;
        }

        private async Task SendVoteForCustomUser(VoteCommand voteCommand, string userId)
        {
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, ApiPath.Votes);
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId);
            httpMessage.Content = voteCommand.ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);
            saveResponse.EnsureSuccessStatusCode();
        }
    }
}
