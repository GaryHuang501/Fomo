using FomoAPI.Domain.Stocks;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios.Portfolios
{
    public class PortfolioTests: IClassFixture<FomoApiApplicationFactory>
    {
        private readonly HttpClient _client;

        public PortfolioTests(FomoApiApplicationFactory clientFactory)
        {
            _client = clientFactory.CreateClientNoAuth("api/portfolios/");
        }

        [Fact]
        public async Task Should_GetNewPortfolioWithCorrectSymbolsAdded()
        {
            var portfolioName = Guid.NewGuid().ToString();

            var payload = new
            {
                name = portfolioName
            }.ToJsonPayload();

            var currentDateTimeUtc = DateTime.UtcNow;

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(string.Empty, payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);
            Assert.True(portfolio.DateCreated > currentDateTimeUtc);
            Assert.Equal(portfolio.Name, portfolioName);

            // Add symbols
            var addTeslaPayload = new
            {
                Ticker = "TSLA",
                Exchange = "NASDAQ"
            }.ToJsonPayload();

            var addJPMPayload = new
            {
                Ticker = "JPM",
                Exchange = "NYSE"
            }.ToJsonPayload();

            var addTeslaResponse =  await _client.PostAsync($"{portfolio.Id}/symbols", addTeslaPayload);

            addTeslaResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await _client.PostAsync($"{portfolio.Id}/symbols", addJPMPayload);

            addJPMResponse.EnsureSuccessStatusCode();

            // Grab portfolio and verify symbols returned
            var getPortfolioResponse = await _client.GetAsync(portfolio.Id.ToString());
            getPortfolioResponse.EnsureSuccessStatusCode();

            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);
            Assert.Equal(2, fetchedPortfolio.Symbols.Count());
            Assert.Contains(fetchedPortfolio.Symbols, s => s.Ticker == "TSLA" && s.ExchangeName == "NASDAQ");
            Assert.Contains(fetchedPortfolio.Symbols, s => s.Ticker == "JPM" && s.ExchangeName == "NYSE");
        }

        [Fact]
        public async Task Should_Return404_WhenAddNonExistingSymbolToPortfolio()
        {
            var payload = new
            {
                name = string.Empty
            }.ToJsonPayload();

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(string.Empty, payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Add Symbols
            var addSymbolPayload = new
            {
                Ticker = "X_!@#",
                Exchange = "NASDAQ"
            }.ToJsonPayload();

            var addSymbolResponse = await _client.PostAsync($"{portfolio.Id}/symbols", addSymbolPayload);

            Assert.Equal(HttpStatusCode.NotFound, addSymbolResponse.StatusCode);
        }

        [Fact]
        public async Task Should_DeletePortfolioWithSymbols()
        {
            var payload = new
            {
                name = string.Empty
            }.ToJsonPayload();

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(string.Empty, payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Add Symbols
            var addTeslaPayload = new
            {
                Ticker = "TSLA",
                Exchange = "NASDAQ"
            }.ToJsonPayload();

            var addTeslaResponse = await _client.PostAsync($"{portfolio.Id}/symbols", addTeslaPayload);

            addTeslaResponse.EnsureSuccessStatusCode();

            var deleteResponse = await _client.DeleteAsync(portfolio.Id.ToString());
            deleteResponse.EnsureSuccessStatusCode();

            // Fetch portoflio to make sure does not exist
            var getPortfolioResponse = await _client.GetAsync(portfolio.Id.ToString());
            Assert.Equal(HttpStatusCode.NotFound, getPortfolioResponse.StatusCode);
        }

        [Fact]
        public async Task Should_RenamePortfolio()
        {
            var payload = new
            {
                name = string.Empty
            }.ToJsonPayload();

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(string.Empty, payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Rename portfolio
            var newName = "newname!";
            var renamePayload = new
            {
                name = newName
            }.ToJsonPayload();

            var renameResponse = await _client.PatchAsync($"{portfolio.Id}/rename", renamePayload);
            renameResponse.EnsureSuccessStatusCode();

            // Grab portfolio and verify renamed
            var getPortfolioResponse = await _client.GetAsync(portfolio.Id.ToString());
            getPortfolioResponse.EnsureSuccessStatusCode();

            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);
            Assert.Equal(newName, fetchedPortfolio.Name);
        }
    }
}
