using FomoAPI.Domain.Stocks;
using System;
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
        public async Task Should_CreatePortfolio()
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
        }
    }
}
