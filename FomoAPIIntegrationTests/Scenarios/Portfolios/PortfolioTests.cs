using FomoAPI.Domain.Stocks;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios.Portfolios
{
    public class PortfolioTests: IClassFixture<FomoApiApplicationFactory>
    {
        private readonly FomoApiApplicationFactory _clientFactory;

        public PortfolioTests(FomoApiApplicationFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private string PortfolioPath(string endpoint = null) => $"api/portfolios/{endpoint}";

        [Fact]
        public async Task Should_GetNewPortfolioWithCorrectSymbolsAdded()
        {
            var client = _clientFactory.CreateClientNoAuth();

            var portfolioName = Guid.NewGuid().ToString();

            var payload = new
            {
                portfolioName
            }.ToJsonPayload();

            var currentDateTimeUtc = DateTime.UtcNow;

            // Create portfolio
            var createPortfolioResponse = await client.PostAsync(PortfolioPath(), payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);
            Assert.True(portfolio.DateCreated > currentDateTimeUtc);
            Assert.Equal(portfolio.Name, portfolioName);

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

            var addTeslaResponse =  await client.PostAsync(PortfolioPath($"{portfolio.Id}/symbols"), addTeslaPayload);

            addTeslaResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await client.PostAsync(PortfolioPath($"{portfolio.Id}/symbols"), addJPMPayload);

            addJPMResponse.EnsureSuccessStatusCode();

            // Grab portfolio and verify symbols returned
            var getPortfolioResponse = await client.GetAsync(PortfolioPath(portfolio.Id.ToString()));
            getPortfolioResponse.EnsureSuccessStatusCode();

            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);
            Assert.Equal(2, fetchedPortfolio.Symbols.Count());
            Assert.Contains(fetchedPortfolio.Symbols, s => s.Ticker == "TSLA" && s.ExchangeName == "NASDAQ");
            Assert.Contains(fetchedPortfolio.Symbols, s => s.Ticker == "JPM" && s.ExchangeName == "NYSE");
        }
    }
}
