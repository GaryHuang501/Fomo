using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        private string PortfolioPath(int? portfolioId = null) => $"api/portfolios/{portfolioId}";

        private string PortfolioSymbolsPath(int portfolioId, int? portfolioSymbolId = null) => $"api/portfolios/{portfolioId}/portfolioSymbols/{portfolioSymbolId}";

        private string PortfolioSymbolsReorderPath(int portfolioId) => $"api/portfolios/{portfolioId}/portfolioSymbols/reorder";

        private string SymbolsSearchPath(string keyword) => $"api/Symbols/?keyword={keyword}";

        [Fact]
        public async Task Should_GetNewPortfolioWithCorrectSymbolsAddedInCorrectOrder()
        {
            var portfolioName = Guid.NewGuid().ToString();

            var payload = new { name = portfolioName }.ToJsonPayload();

            var currentDateTimeUtc = DateTime.UtcNow;

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);
            Assert.True(portfolio.DateCreated > currentDateTimeUtc);
            Assert.Equal(portfolio.Name, portfolioName);

            // Search for symbols by keyword
            var searchTSLASymbolResponse = await _client.GetAsync(SymbolsSearchPath("TSLA"));
            searchTSLASymbolResponse.EnsureSuccessStatusCode();

            var searchJPMSymbolResponse = await _client.GetAsync(SymbolsSearchPath("JPM"));
            searchJPMSymbolResponse.EnsureSuccessStatusCode();

            var tslaSymbol = (await searchTSLASymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();
            var jpmSymbol = (await searchJPMSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();

            // Add symbols to portfolio
            var addTSLAResponse =  await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.Id }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.Id }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();

            var tslaPortfolioSymbol = await addTSLAResponse.Content.ReadAsAsync<PortfolioSymbol>();
            Assert.Equal("NASDAQ", tslaPortfolioSymbol.ExchangeName);
            Assert.Equal("TSLA", tslaPortfolioSymbol.Ticker);
            Assert.False(string.IsNullOrWhiteSpace(tslaPortfolioSymbol.FullName));
            Assert.Equal(1, tslaPortfolioSymbol.SortOrder);
            Assert.True(tslaPortfolioSymbol.Id > 0);

            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();
            Assert.Equal("NYSE", jpmPortfolioSymbol.ExchangeName);
            Assert.Equal("JPM", jpmPortfolioSymbol.Ticker);
            Assert.False(string.IsNullOrWhiteSpace(jpmPortfolioSymbol.FullName));
            Assert.Equal(2, jpmPortfolioSymbol.SortOrder);
            Assert.True(jpmPortfolioSymbol.Id > 0);

            // Grab portfolio and verify symbols returned
            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            getPortfolioResponse.EnsureSuccessStatusCode();

            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);
            Assert.Equal(2, fetchedPortfolio.Symbols.Count());

            var tsla = fetchedPortfolio.Symbols.Single(s => s.Ticker == "TSLA");
            var jpm = fetchedPortfolio.Symbols.Single(s => s.Ticker == "JPM");

            Assert.Equal("NASDAQ", tsla.ExchangeName);
            Assert.False(string.IsNullOrWhiteSpace(tsla.FullName));
            Assert.Equal(1, tsla.SortOrder);
            Assert.Equal(tslaSymbol.Id, tsla.SymbolId);

            Assert.Equal("NYSE", jpm.ExchangeName);
            Assert.False(string.IsNullOrWhiteSpace(jpm.FullName));
            Assert.Equal(2, jpm.SortOrder);
            Assert.Equal(jpmSymbol.Id, jpm.SymbolId);
        }

        [Fact]
        public async Task Should_ReturnBadRequestWhenSymbolAlreadyExistsInPortfolio()
        {
            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();
            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            var searchTSLASymbolResponse = await _client.GetAsync(SymbolsSearchPath("TSLA"));
            searchTSLASymbolResponse.EnsureSuccessStatusCode();

            var tslaSymbol = (await searchTSLASymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();

            // Add symbols to portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.Id }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addTSLAResponse2 = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.Id }.ToJsonPayload());
            Assert.Equal(HttpStatusCode.BadRequest, addTSLAResponse2.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenAddNonExistingSymbolToPortfolio()
        {
            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();
            Assert.True(portfolio.Id > 0);

            // Add symbols to portfolio
            var addInvalidSymbolResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = -999 }.ToJsonPayload());

            Assert.Equal(HttpStatusCode.BadRequest, addInvalidSymbolResponse.StatusCode); 
        }

        [Fact]
        public async Task Should_DeletePortfolioWithSymbols()
        {
            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            var searchJPMSymbolResponse = await _client.GetAsync(SymbolsSearchPath("JPM"));
            searchJPMSymbolResponse.EnsureSuccessStatusCode();

            var jpmSymbol = (await searchJPMSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();

            // Add symbols to portfolio
            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.Id }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();

            // Verify Portfolio exists
            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            getPortfolioResponse.EnsureSuccessStatusCode();

            // Delete and check it no longer exists
            var deletePortfolioResponse = await _client.DeleteAsync(PortfolioPath(portfolio.Id));
            deletePortfolioResponse.EnsureSuccessStatusCode();

            var getPortfolioAfterDeleteResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            Assert.Equal(HttpStatusCode.NotFound, getPortfolioAfterDeleteResponse.StatusCode);
        }

        [Fact]
        public async Task Should_DeleteSymbolFromPortfolio()
        {
            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            var searchTSLASymbolResponse = await _client.GetAsync(SymbolsSearchPath("TSLA"));
            searchTSLASymbolResponse.EnsureSuccessStatusCode();

            var searchJPMSymbolResponse = await _client.GetAsync(SymbolsSearchPath("JPM"));
            searchJPMSymbolResponse.EnsureSuccessStatusCode();

            var tslaSymbol = (await searchTSLASymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();
            var jpmSymbol = (await searchJPMSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();

            // Add symbols to portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.Id }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.Id }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();

            // Delete JPM
            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();
            var deleteJPMResponse = await _client.DeleteAsync(PortfolioSymbolsPath(portfolio.Id, jpmPortfolioSymbol.Id));
            deleteJPMResponse.EnsureSuccessStatusCode();

            // Check only one symbol left on portfolio
            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            getPortfolioResponse.EnsureSuccessStatusCode();
            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Single(fetchedPortfolio.Symbols);
            Assert.Equal("TSLA", fetchedPortfolio.Symbols.First().Ticker);
        }

        [Fact]
        public async Task Should_RenamePortfolio()
        {
            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Rename portfolio
            var newName = "newname!";
            var renamePayload = new { name = newName }.ToJsonPayload();

            var renameResponse = await _client.PatchAsync(PortfolioPath(portfolio.Id) + "/rename", renamePayload);
            renameResponse.EnsureSuccessStatusCode();

            // Grab portfolio and verify renamed
            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            getPortfolioResponse.EnsureSuccessStatusCode();

            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);
            Assert.Equal(newName, fetchedPortfolio.Name);
        }

        [Fact]
        public async Task Should_ReorderPortfolioSymbols()
        {
            var payload = new { name = "" }.ToJsonPayload();

            // Create portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            var searchTSLASymbolResponse = await _client.GetAsync(SymbolsSearchPath("TSLA"));
            searchTSLASymbolResponse.EnsureSuccessStatusCode();

            var searchJPMSymbolResponse = await _client.GetAsync(SymbolsSearchPath("JPM"));
            searchJPMSymbolResponse.EnsureSuccessStatusCode();

            var searchFBSymbolResponse = await _client.GetAsync(SymbolsSearchPath("FB"));
            searchFBSymbolResponse.EnsureSuccessStatusCode();

            var searchMSFTSymbolResponse = await _client.GetAsync(SymbolsSearchPath("MSFT"));
            searchMSFTSymbolResponse.EnsureSuccessStatusCode();

            var tslaSymbol = (await searchTSLASymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();
            var jpmSymbol = (await searchJPMSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();
            var fbSymbol = (await searchFBSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();
            var msftSymbol = (await searchMSFTSymbolResponse.Content.ReadAsAsync<IEnumerable<Symbol>>()).First();

            // Add symbols to portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.Id }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();
            var tslaPortfolioSymbol = await addTSLAResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.Id }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();
            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addFBResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = fbSymbol.Id }.ToJsonPayload());
            addFBResponse.EnsureSuccessStatusCode();
            var fbPortfolioSymbol = await addFBResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addMSFTResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = msftSymbol.Id }.ToJsonPayload());
            addMSFTResponse.EnsureSuccessStatusCode();
            var msftPortfolioSymbol = await addMSFTResponse.Content.ReadAsAsync<PortfolioSymbol>();

            // Check starting order is as expected

            Assert.Equal(1, tslaPortfolioSymbol.SortOrder);
            Assert.Equal(2, jpmPortfolioSymbol.SortOrder);
            Assert.Equal(3, fbPortfolioSymbol.SortOrder);
            Assert.Equal(4, msftPortfolioSymbol.SortOrder);

            var newOrder = new
            {
                PortfolioSymbolIdToSortOrder = new Dictionary<int, int>
                {
                    { fbPortfolioSymbol.Id, 1 },
                    { tslaPortfolioSymbol.Id, 2 },
                    { msftPortfolioSymbol.Id, 3 },
                    { jpmPortfolioSymbol.Id, 4},
                }

            }.ToJsonPayload();

            var reorderTSLAResponse = await _client.PatchAsync(PortfolioSymbolsReorderPath(portfolio.Id), newOrder);

            reorderTSLAResponse.EnsureSuccessStatusCode();

            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Equal(portfolio.Id, fetchedPortfolio.Id);

            var tsla = fetchedPortfolio.Symbols.Single(s => s.Ticker == "TSLA");
            var jpm = fetchedPortfolio.Symbols.Single(s => s.Ticker == "JPM");
            var fb = fetchedPortfolio.Symbols.Single(s => s.Ticker == "FB");
            var msft = fetchedPortfolio.Symbols.Single(s => s.Ticker == "MSFT");

            Assert.Equal(1, fb.SortOrder);
            Assert.Equal(2, tsla.SortOrder);
            Assert.Equal(3, msft.SortOrder);
            Assert.Equal(4, jpm.SortOrder);
        }
    }
}
