﻿using FomoAPI.Application.DTOs;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Enums;
using FomoAPIIntegrationTests.Fixtures;
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
    public class PortfolioTests: IClassFixture<ExchangeSyncSetupFixture>
    {
        private static HttpClient _client = (new FomoApiApplicationFactory()).CreateClientNoAuth("api/Portfolios/");

        public PortfolioTests()
        {
        }

        private string PortfolioPath(int? portfolioId = null) => $"api/Portfolios/{portfolioId}";

        private string PortfolioSymbolsPath(int portfolioId, int? portfolioSymbolId = null) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/{portfolioSymbolId}";

        private string PortfolioSymbolsReorderPath(int portfolioId) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/reorder";

        private string SymbolSearchPath(string ticker, int limit) => $"api/Symbols/?keywords={ticker}&limit={limit}";

        [Fact]
        public async Task Should_GetNewPortfolioWithCorrectSymbolsAddedInCorrectOrder()
        {
            var portfolioName = Guid.NewGuid().ToString();

            var payload = new { name = portfolioName }.ToJsonPayload();

            var currentDateTimeUtc = DateTime.UtcNow;

            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);
            Assert.True(portfolio.DateCreated > currentDateTimeUtc);
            Assert.Equal(portfolio.Name, portfolioName);

            // Search for symbols by keyword
            var tslaSymbol = await FetchSymbol("TSLA", ExchangeType.NASDAQ);
            var jpmSymbol = await FetchSymbol("JPM", ExchangeType.NYSE);

            // Add symbols to Portfolio
            var addTSLAResponse =  await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.SymbolId }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.SymbolId }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();

            var tslaPortfolioSymbol = await addTSLAResponse.Content.ReadAsAsync<PortfolioSymbol>();
            Assert.Equal("NASDAQ", tslaPortfolioSymbol.ExchangeName);
            Assert.Equal("TSLA", tslaPortfolioSymbol.Ticker);
            Assert.False(string.IsNullOrWhiteSpace(tslaPortfolioSymbol.FullName));
            Assert.Equal(1, tslaPortfolioSymbol.SortOrder);
            Assert.True(tslaPortfolioSymbol.SymbolId > 0);

            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();
            Assert.Equal("NYSE", jpmPortfolioSymbol.ExchangeName);
            Assert.Equal("JPM", jpmPortfolioSymbol.Ticker);
            Assert.False(string.IsNullOrWhiteSpace(jpmPortfolioSymbol.FullName));
            Assert.Equal(2, jpmPortfolioSymbol.SortOrder);
            Assert.True(jpmPortfolioSymbol.SymbolId > 0);

            // Grab Portfolio and verify symbols returned
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
            Assert.Equal(tslaSymbol.SymbolId, tsla.SymbolId);

            Assert.Equal("NYSE", jpm.ExchangeName);
            Assert.False(string.IsNullOrWhiteSpace(jpm.FullName));
            Assert.Equal(2, jpm.SortOrder);
            Assert.Equal(jpmSymbol.SymbolId, jpm.SymbolId);
        }

        [Fact]
        public async Task Should_ReturnBadRequestWhenSymbolAlreadyExistsInPortfolio()
        {
            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();
            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            SymbolSearchResultDTO tslaSymbol = await FetchSymbol("TSLA", ExchangeType.NASDAQ);

            // Add symbols to Portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.SymbolId }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addTSLAResponse2 = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.SymbolId }.ToJsonPayload());
            Assert.Equal(HttpStatusCode.BadRequest, addTSLAResponse2.StatusCode);
        }

        [Fact]
        public async Task Should_ReturnBadRequest_WhenAddNonExistingSymbolToPortfolio()
        {
            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();
            Assert.True(portfolio.Id > 0);

            // Add symbols to Portfolio
            var addInvalidSymbolResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = -999 }.ToJsonPayload());

            Assert.Equal(HttpStatusCode.BadRequest, addInvalidSymbolResponse.StatusCode); 
        }

        [Fact]
        public async Task Should_DeletePortfolioWithSymbols()
        {
            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            SymbolSearchResultDTO jpmSymbol = await FetchSymbol("JPM", ExchangeType.NYSE);

            // Add symbols to Portfolio
            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.SymbolId }.ToJsonPayload());
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
            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            SymbolSearchResultDTO tslaSymbol = await FetchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO jpmSymbol = await FetchSymbol("JPM", ExchangeType.NYSE);

            // Add symbols to Portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.SymbolId }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.SymbolId }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();

            // Delete JPM
            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();
            var deleteJPMResponse = await _client.DeleteAsync(PortfolioSymbolsPath(portfolio.Id, jpmPortfolioSymbol.Id));
            deleteJPMResponse.EnsureSuccessStatusCode();

            // Check only one symbol left on Portfolio
            var getPortfolioResponse = await _client.GetAsync(PortfolioPath(portfolio.Id));
            getPortfolioResponse.EnsureSuccessStatusCode();
            var fetchedPortfolio = await getPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.Single(fetchedPortfolio.Symbols);
            Assert.Equal("TSLA", fetchedPortfolio.Symbols.First().Ticker);
        }

        [Fact]
        public async Task Should_RenamePortfolio()
        {
            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), new { name = string.Empty }.ToJsonPayload());
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Rename Portfolio
            var newName = "newname!";
            var renamePayload = new { name = newName }.ToJsonPayload();

            var renameResponse = await _client.PatchAsync(PortfolioPath(portfolio.Id) + "/rename", renamePayload);
            renameResponse.EnsureSuccessStatusCode();

            // Grab Portfolio and verify renamed
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

            // Create Portfolio
            var createPortfolioResponse = await _client.PostAsync(PortfolioPath(), payload);
            createPortfolioResponse.EnsureSuccessStatusCode();

            var portfolio = await createPortfolioResponse.Content.ReadAsAsync<Portfolio>();

            Assert.True(portfolio.Id > 0);

            // Search for symbols by keyword
            SymbolSearchResultDTO tslaSymbol = await FetchSymbol("TSLA", ExchangeType.NASDAQ);
            SymbolSearchResultDTO jpmSymbol = await FetchSymbol("JPM", ExchangeType.NYSE);
            SymbolSearchResultDTO fbSymbol = await FetchSymbol("FB", ExchangeType.NASDAQ);
            SymbolSearchResultDTO msftSymbol = await FetchSymbol("MSFT", ExchangeType.NASDAQ);

            // Add symbols to Portfolio
            var addTSLAResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = tslaSymbol.SymbolId }.ToJsonPayload());
            addTSLAResponse.EnsureSuccessStatusCode();
            var tslaPortfolioSymbol = await addTSLAResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addJPMResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = jpmSymbol.SymbolId }.ToJsonPayload());
            addJPMResponse.EnsureSuccessStatusCode();
            var jpmPortfolioSymbol = await addJPMResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addFBResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = fbSymbol.SymbolId }.ToJsonPayload());
            addFBResponse.EnsureSuccessStatusCode();
            var fbPortfolioSymbol = await addFBResponse.Content.ReadAsAsync<PortfolioSymbol>();

            var addMSFTResponse = await _client.PostAsync(PortfolioSymbolsPath(portfolio.Id), new { SymbolId = msftSymbol.SymbolId }.ToJsonPayload());
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

        private async Task<SymbolSearchResultDTO> FetchSymbol(string ticker, ExchangeType exchange)
        {
            await Task.Delay(100); // To prevent hammering stock client

            var response = await _client.GetAsync(SymbolSearchPath(ticker, 1));
            response.EnsureSuccessStatusCode();
            var searchResults = await response.Content.ReadAsAsync<IEnumerable<SymbolSearchResultDTO>>();

            var result = searchResults.Single();

            Assert.Equal(ticker, result.Symbol);

            return searchResults.Single();
        }
    }
}
