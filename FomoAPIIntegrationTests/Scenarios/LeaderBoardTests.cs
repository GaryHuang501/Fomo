using Dapper;
using FomoAPI.Application.Commands.Vote;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.ViewModels;
using FomoAPI.Application.ViewModels.LeaderBoard;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Infrastructure.Stocks;
using FomoAPIIntegrationTests.Fixtures;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    public class LeaderBoardTests : IClassFixture<ExchangeSyncSetupFixture>, IClassFixture<FomoApiFixture>, IAsyncLifetime
    { 
        private readonly HttpClient _client;

        private class TestUser
        {
            public Guid UserId { get; set; }

            public int PortfolioId { get; set; }

            public string Name { get; set; }

        }

        public LeaderBoardTests(FomoApiFixture webApiFactoryFixture)
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
            using var connection = new SqlConnection(AppTestSettings.Instance.TestDBConnectionString);
            await connection.ExecuteAsync("DELETE FROM Vote", null);
            await connection.ExecuteAsync("DELETE FROM PortfolioSymbol", null);
            await connection.ExecuteAsync("DELETE FROM Portfolio", null);
            await connection.ExecuteAsync("DELETE FROM SingleQuoteData", null);
            await connection.ExecuteAsync("DELETE FROM AspNetUsers", null);
        }

        [Fact]
        public async Task Should_HandleEmptyData()
        {
            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(100));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            Assert.Empty(leaderBoardViewModel.WorstPerformers.Values); 
            Assert.Empty(leaderBoardViewModel.BestPerformers.Values);
            Assert.Empty(leaderBoardViewModel.MostBearish.Values);
            Assert.Empty(leaderBoardViewModel.MostBullish.Values);
        }

        [Fact]
        public async Task Should_SetTheCorrectColumnHeaders()
        {
            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(100));
            var leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            Assert.Equal("Worst Performing Users", leaderBoardViewModel.WorstPerformers.Header);
            Assert.Equal("Best Performing Users", leaderBoardViewModel.BestPerformers.Header);
            Assert.Equal("Most Bearish Stocks", leaderBoardViewModel.MostBearish.Header);
            Assert.Equal("Most Bullish Stocks", leaderBoardViewModel.MostBullish.Header);
        }

        [Fact]
        public async Task Should_ReturnMostBearishAndMostBullishStocksByVotes()
        {
            TestUser testUser1 = await CreateTestUserWithPortfolio();
            TestUser testUser2 = await CreateTestUserWithPortfolio();
            TestUser testUser3 = await CreateTestUserWithPortfolio();

            SymbolSearchResultDTO jpmStock = await FetchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO metaStock = await FetchSymbol("META", ExchangeType.NASDAQ);
            SymbolSearchResultDTO tslaStock = await FetchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO msftStock = await FetchSymbol("MSFT", ExchangeType.NASDAQ);

            // User 1 votes
            await SubmitVoteForUser(new VoteCommand { SymbolId = jpmStock.SymbolId, Direction = VoteDirection.UpVote }, testUser1.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = metaStock.SymbolId, Direction = VoteDirection.UpVote }, testUser1.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = tslaStock.SymbolId, Direction = VoteDirection.DownVote }, testUser1.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = msftStock.SymbolId, Direction = VoteDirection.DownVote }, testUser1.UserId);

            // User 2 votes
            await SubmitVoteForUser(new VoteCommand { SymbolId = jpmStock.SymbolId, Direction = VoteDirection.UpVote }, testUser2.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = metaStock.SymbolId, Direction = VoteDirection.DownVote }, testUser2.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = tslaStock.SymbolId, Direction = VoteDirection.DownVote }, testUser2.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = msftStock.SymbolId, Direction = VoteDirection.DownVote }, testUser2.UserId);

            // User 2 votes
            await SubmitVoteForUser(new VoteCommand { SymbolId = jpmStock.SymbolId, Direction = VoteDirection.UpVote }, testUser3.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = metaStock.SymbolId, Direction = VoteDirection.None }, testUser3.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = tslaStock.SymbolId, Direction = VoteDirection.DownVote }, testUser3.UserId);
            await SubmitVoteForUser(new VoteCommand { SymbolId = msftStock.SymbolId, Direction = VoteDirection.UpVote }, testUser3.UserId);

            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(4));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            // Verify most bullish order
            BoardValue mostBullishRank1 = leaderBoardViewModel.MostBullish.Values.ElementAt(0);
            BoardValue mostBullishRank2 = leaderBoardViewModel.MostBullish.Values.ElementAt(1);
            BoardValue mostBullishRank3 = leaderBoardViewModel.MostBullish.Values.ElementAt(2);
            BoardValue mostBullishRank4 = leaderBoardViewModel.MostBullish.Values.ElementAt(3);

            Assert.Equal(jpmStock.SymbolId.ToString(), mostBullishRank1.Id);
            Assert.Equal("JPM", mostBullishRank1.Name);
            Assert.Equal("3", mostBullishRank1.Value);

            Assert.Equal(metaStock.SymbolId.ToString(), mostBullishRank2.Id);
            Assert.Equal("META", mostBullishRank2.Name);
            Assert.Equal("0", mostBullishRank2.Value);

            Assert.Equal(msftStock.SymbolId.ToString(), mostBullishRank3.Id);
            Assert.Equal("MSFT", mostBullishRank3.Name);
            Assert.Equal("-1", mostBullishRank3.Value);

            Assert.Equal(tslaStock.SymbolId.ToString(), mostBullishRank4.Id);
            Assert.Equal("TSLA", mostBullishRank4.Name);
            Assert.Equal("-3", mostBullishRank4.Value);

            // Verify most bearish order
            BoardValue mostBearishRank1 = leaderBoardViewModel.MostBearish.Values.ElementAt(0);
            BoardValue mostBearishRank2 = leaderBoardViewModel.MostBearish.Values.ElementAt(1);
            BoardValue mostBearishRank3 = leaderBoardViewModel.MostBearish.Values.ElementAt(2);
            BoardValue mostBearishRank4 = leaderBoardViewModel.MostBearish.Values.ElementAt(3);

            Assert.Equal(tslaStock.SymbolId.ToString(), mostBearishRank1.Id);
            Assert.Equal("TSLA", mostBearishRank1.Name);
            Assert.Equal("-3", mostBearishRank1.Value);

            Assert.Equal(msftStock.SymbolId.ToString(), mostBearishRank2.Id);
            Assert.Equal("MSFT", mostBearishRank2.Name);
            Assert.Equal("-1", mostBearishRank2.Value);

            Assert.Equal(metaStock.SymbolId.ToString(), mostBearishRank3.Id);
            Assert.Equal("META", mostBearishRank3.Name);
            Assert.Equal("0", mostBearishRank3.Value);

            Assert.Equal(jpmStock.SymbolId.ToString(), mostBearishRank4.Id);
            Assert.Equal("JPM", mostBearishRank4.Name);
            Assert.Equal("3", mostBearishRank4.Value);
        }

        [Fact]
        public async Task Should_ReturnBestAndWorstPerformers()
        {
            TestUser testUser1 = await CreateTestUserWithPortfolio();
            TestUser testUser2 = await CreateTestUserWithPortfolio();
            TestUser testUser3 = await CreateTestUserWithPortfolio();

            SymbolSearchResultDTO jpmStock = await FetchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO metaStock = await FetchSymbol("META", ExchangeType.NASDAQ);
            SymbolSearchResultDTO tslaStock = await FetchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO msftStock = await FetchSymbol("MSFT", ExchangeType.NASDAQ);

            // Add singlequote stocks
            var jpmData = new SingleQuoteData(jpmStock.SymbolId, jpmStock.Ticker, 200, 0, 0, DateTime.UtcNow);
            var fbData = new SingleQuoteData(metaStock.SymbolId, metaStock.Ticker, 333.33m, 0, 0, DateTime.UtcNow);
            var tslaData = new SingleQuoteData(tslaStock.SymbolId, tslaStock.Ticker, 888.88m, 0, 0, DateTime.UtcNow);
            var msftData = new SingleQuoteData(msftStock.SymbolId, msftStock.Ticker, 225, 0, 0, DateTime.UtcNow);

            await SetSingleQuoteData(new UpsertSingleQuoteData(jpmStock.SymbolId, jpmData));
            await SetSingleQuoteData(new UpsertSingleQuoteData(fbData.SymbolId, fbData));
            await SetSingleQuoteData(new UpsertSingleQuoteData(tslaData.SymbolId, tslaData));
            await SetSingleQuoteData(new UpsertSingleQuoteData(msftData.SymbolId, msftData));

            // Add symbols User 1 
            await AddSymbolToPortfolioWithAvgPrice(testUser1, jpmStock.SymbolId, 100.25m); 
            await AddSymbolToPortfolioWithAvgPrice(testUser1, metaStock.SymbolId, 260.35m);
            await AddSymbolToPortfolioWithAvgPrice(testUser1, msftStock.SymbolId, 175.50m);

            // Add symbols User 2
            await AddSymbolToPortfolioWithAvgPrice(testUser2, jpmStock.SymbolId, 5.00m);
            await AddSymbolToPortfolioWithAvgPrice(testUser2, metaStock.SymbolId, 10.00m);

            // Add Symbols Users 3 
            await AddSymbolToPortfolioWithAvgPrice(testUser3, jpmStock.SymbolId, 180.88m);
            await AddSymbolToPortfolioWithAvgPrice(testUser3, metaStock.SymbolId, 400.00m);
            await AddSymbolToPortfolioWithAvgPrice(testUser3, tslaStock.SymbolId, 1000m);
            await AddSymbolToPortfolioWithAvgPrice(testUser3, msftStock.SymbolId, 300.90m);

            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(3));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            // Verify calculations
            BoardValue bestPerformerRank1 = leaderBoardViewModel.BestPerformers.Values.ElementAt(0);
            BoardValue bestPerformerRank2 = leaderBoardViewModel.BestPerformers.Values.ElementAt(1);
            BoardValue bestPerformerRank3 = leaderBoardViewModel.BestPerformers.Values.ElementAt(2);

            Assert.Equal(3, leaderBoardViewModel.BestPerformers.Values.Count());

            Assert.Equal(testUser2.UserId, Guid.Parse(bestPerformerRank1.Id)); 
            Assert.Equal(testUser1.UserId, Guid.Parse(bestPerformerRank2.Id));
            Assert.Equal(testUser3.UserId, Guid.Parse(bestPerformerRank3.Id));

            Assert.Equal(testUser2.UserId, Guid.Parse(bestPerformerRank1.Name));
            Assert.Equal(testUser1.UserId, Guid.Parse(bestPerformerRank2.Name));
            Assert.Equal(testUser3.UserId, Guid.Parse(bestPerformerRank3.Name));

            Assert.Equal("3,566.65", bestPerformerRank1.Value);
            Assert.Equal("51.91", bestPerformerRank2.Value);
            Assert.Equal("-10.61", bestPerformerRank3.Value);

            BoardValue worstPerformerRank1 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(0);
            BoardValue worstPerformerRank2 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(1);
            BoardValue worstPerformerRank3 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(2);

            Assert.Equal(3, leaderBoardViewModel.WorstPerformers.Values.Count());

            Assert.Equal(testUser3.UserId, Guid.Parse(worstPerformerRank1.Id));
            Assert.Equal(testUser1.UserId, Guid.Parse(worstPerformerRank2.Id));
            Assert.Equal(testUser2.UserId, Guid.Parse(worstPerformerRank3.Id));

            Assert.Equal("-10.61", worstPerformerRank1.Value);
            Assert.Equal("51.91", worstPerformerRank2.Value);
            Assert.Equal("3,566.65", worstPerformerRank3.Value);
        }

        [Fact]
        public async Task Should_IgnorePortfolioSymbolsWithNoAveragePrice()
        {
            TestUser testUser1 = await CreateTestUserWithPortfolio();
            TestUser testUser2 = await CreateTestUserWithPortfolio();

            SymbolSearchResultDTO jpmStock = await FetchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO metaStock = await FetchSymbol("META", ExchangeType.NASDAQ);

            // Add singlequote stocks
            var jpmData = new SingleQuoteData(jpmStock.SymbolId, jpmStock.Ticker, 200, 0, 0, DateTime.UtcNow);
            var fbData = new SingleQuoteData(metaStock.SymbolId, metaStock.Ticker, 333.33m, 0, 0, DateTime.UtcNow);

            await SetSingleQuoteData(new UpsertSingleQuoteData(jpmStock.SymbolId, jpmData));
            await SetSingleQuoteData(new UpsertSingleQuoteData(fbData.SymbolId, fbData));

            // Add symbols User 1 
            await AddSymbolToPortfolioWithAvgPrice(testUser1, jpmStock.SymbolId, 100);
            await AddSymbolToPortfolio(testUser1, metaStock.SymbolId);

            // Add symbols User 2
            await AddSymbolToPortfolio(testUser2, jpmStock.SymbolId);
            await AddSymbolToPortfolio(testUser2, metaStock.SymbolId);

            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(2));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            // Verify calculations
            BoardValue bestPerformerRank1 = leaderBoardViewModel.BestPerformers.Values.ElementAt(0);
            BoardValue worstPerformerRank1 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(0);

            Assert.Single(leaderBoardViewModel.BestPerformers.Values);
            Assert.Single(leaderBoardViewModel.WorstPerformers.Values);

            Assert.Equal(testUser1.UserId, Guid.Parse(bestPerformerRank1.Id));
            Assert.Equal("100.00", bestPerformerRank1.Value);

            Assert.Equal(testUser1.UserId, Guid.Parse(worstPerformerRank1.Id));
            Assert.Equal("100.00", worstPerformerRank1.Value);
        }

        [Fact]
        public async Task Should_IgnorePortfolioSymbolsWithNoQuoteData()
        {
            TestUser testUser1 = await CreateTestUserWithPortfolio();
            TestUser testUser2 = await CreateTestUserWithPortfolio();

            SymbolSearchResultDTO jpmStock = await FetchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO metaStock = await FetchSymbol("META", ExchangeType.NASDAQ);

            // Add singlequote stocks
            var jpmData = new SingleQuoteData(jpmStock.SymbolId, jpmStock.Ticker, 200, 0, 0, DateTime.UtcNow);

            await SetSingleQuoteData(new UpsertSingleQuoteData(jpmStock.SymbolId, jpmData));

            // Add symbols User 1 
            await AddSymbolToPortfolioWithAvgPrice(testUser1, jpmStock.SymbolId, 100);
            await AddSymbolToPortfolioWithAvgPrice(testUser1, metaStock.SymbolId, 200);

            // Add symbols User 2
            await AddSymbolToPortfolioWithAvgPrice(testUser2, metaStock.SymbolId, 200);

            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(2));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            // Verify calculations
            BoardValue bestPerformerRank1 = leaderBoardViewModel.BestPerformers.Values.ElementAt(0);
            BoardValue worstPerformerRank1 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(0);

            Assert.Single(leaderBoardViewModel.BestPerformers.Values);
            Assert.Single(leaderBoardViewModel.WorstPerformers.Values);

            Assert.Equal(testUser1.UserId, Guid.Parse(bestPerformerRank1.Id));
            Assert.Equal("100.00", bestPerformerRank1.Value);

            Assert.Equal(testUser1.UserId, Guid.Parse(worstPerformerRank1.Id));
            Assert.Equal("100.00", worstPerformerRank1.Value);
        }

        [Fact]
        public async Task Should_OnlyShowedTheTopMembers()
        {
            TestUser testUser1 = await CreateTestUserWithPortfolio();
            TestUser testUser2 = await CreateTestUserWithPortfolio();
            TestUser testUser3 = await CreateTestUserWithPortfolio();

            SymbolSearchResultDTO jpmStock = await FetchSymbol("JPM", ExchangeType.NYSE);

            // Add singlequote stocks
            var jpmData = new SingleQuoteData(jpmStock.SymbolId, jpmStock.Ticker, 200, 0, 0, DateTime.UtcNow);

            await SetSingleQuoteData(new UpsertSingleQuoteData(jpmStock.SymbolId, jpmData));

            // Add symbols User 1 
            await AddSymbolToPortfolioWithAvgPrice(testUser1, jpmStock.SymbolId, 100);

            // Add symbols User 2
            await AddSymbolToPortfolioWithAvgPrice(testUser2, jpmStock.SymbolId, 200);

            // Add symbols User 3
            await AddSymbolToPortfolioWithAvgPrice(testUser3, jpmStock.SymbolId, 300);

            var leaderBoardResponse = await _client.GetAsync(ApiPath.LeaderBoard(2));
            LeaderBoardViewModel leaderBoardViewModel = await leaderBoardResponse.Content.ReadAsAsync<LeaderBoardViewModel>();

            // Verify calculations
            BoardValue bestPerformerRank1 = leaderBoardViewModel.BestPerformers.Values.ElementAt(0);
            BoardValue bestPerformerRank2 = leaderBoardViewModel.BestPerformers.Values.ElementAt(1);

            BoardValue worstPerformerRank1 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(0);
            BoardValue worstPerformerRank2 = leaderBoardViewModel.WorstPerformers.Values.ElementAt(1);

            Assert.Equal(2, leaderBoardViewModel.BestPerformers.Values.Count());
            Assert.Equal(2, leaderBoardViewModel.WorstPerformers.Values.Count());

            Assert.Equal(testUser1.UserId, Guid.Parse(bestPerformerRank1.Id));
            Assert.Equal(testUser2.UserId, Guid.Parse(bestPerformerRank2.Id));

            Assert.Equal(testUser3.UserId, Guid.Parse(worstPerformerRank1.Id));
            Assert.Equal(testUser2.UserId, Guid.Parse(worstPerformerRank2.Id));
        }

        private async Task SetSingleQuoteData(UpsertSingleQuoteData data)
        {
            var repo = new StockDataRepository(AppTestSettings.Instance.GetDbOptionsMonitor());

            await repo.UpsertSingleQuoteData(data);
        }

        private async Task AddSymbolToPortfolio(TestUser testUser, int symbolId)
        {
            var addResponse = await _client.PostAsync(ApiPath.PortfolioSymbols(testUser.PortfolioId), new { SymbolId = symbolId }.ToJsonPayload());
            addResponse.EnsureSuccessStatusCode();

            await addResponse.Content.ReadAsAsync<PortfolioSymbol>();
        }

        private async Task AddSymbolToPortfolioWithAvgPrice(TestUser testUser, int symbolId, decimal averagePrice) {

            var addResponse = await _client.PostAsync(ApiPath.PortfolioSymbols(testUser.PortfolioId), new { SymbolId = symbolId }.ToJsonPayload());
            addResponse.EnsureSuccessStatusCode();

            PortfolioSymbol portfolioSymbol = await addResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var httpMessage = new HttpRequestMessage(HttpMethod.Patch, ApiPath.PortfolioSymbols(testUser.PortfolioId, portfolioSymbol.Id));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, testUser.UserId.ToString());
            httpMessage.Content = (new object[] { new { op = "replace", path = "/averagePrice", value = averagePrice } }).ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);
            saveResponse.EnsureSuccessStatusCode();
        }

        private async Task SubmitVoteForUser(VoteCommand voteCommand, Guid userId)
        {
            var httpMessage = new HttpRequestMessage(HttpMethod.Post, ApiPath.Votes);
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId.ToString());
            httpMessage.Content = voteCommand.ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);
            saveResponse.EnsureSuccessStatusCode();
        }

        private async Task<TestUser> CreateTestUserWithPortfolio()
        {
            Guid userId = await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString);

            var httpMessage = new HttpRequestMessage(HttpMethod.Post, ApiPath.Portfolio());
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId.ToString());
            httpMessage.Content = (new { name = "test" }).ToJsonPayload();

            var createPortfolioResponse = await _client.SendAsync(httpMessage);
            createPortfolioResponse.EnsureSuccessStatusCode();
            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            return new TestUser
            {
                UserId = userId,
                PortfolioId = portfolio.Id,
                Name = userId.ToString()
            };
        }

        private async Task<SymbolSearchResultDTO> FetchSymbol(string ticker, ExchangeType exchange)
        {
            var response = await _client.GetAsync(ApiPath.SymbolSearch(ticker, 1));
            response.EnsureSuccessStatusCode();
            var searchResults = await response.Content.ReadAsAsync<IEnumerable<SymbolSearchResultDTO>>();

            var result = searchResults.Single();

            Assert.Equal(ticker, result.Ticker);

            return searchResults.Single();
        }
    }
}
