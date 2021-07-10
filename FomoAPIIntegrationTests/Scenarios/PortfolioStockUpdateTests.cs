using FomoAPI.Application.Queries;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using FomoAPIIntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    public class PortfolioStockUpdateTests : IAsyncLifetime, IClassFixture<ExchangeSyncSetupFixture>
    {

        private readonly ISymbolRepository _symbolRepo;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IStockDataRepository _stockRepo;

        public PortfolioStockUpdateTests()
        {
            var mockDbOptions = AppTestSettings.Instance.GetDbOptionsMonitor();

            _symbolRepo = new SymbolRepository(mockDbOptions);
            _portfolioRepo = new PortfolioRepository(mockDbOptions);
            _stockRepo = new StockDataRepository(mockDbOptions);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task InitializeAsync()
        {
            await TestUtil.ClearUserData();
            await TestUtil.ClearStockData();
        }

        [Fact]
        public async Task Query_ShouldReturnOutdatedPortfolioSymbols_MultiplePortfolio()
        {
            var jpm = await _symbolRepo.GetSymbol("JPM");
            var ba = await _symbolRepo.GetSymbol("BA");
            var bac = await _symbolRepo.GetSymbol("bac");

            var portfolio1 = await CreateUserAndPortfolio("p1");
            var portfolio2 = await CreateUserAndPortfolio("p2");
            var portfolio3 = await CreateUserAndPortfolio("p3");

            // Add symbols for user 1
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, ba.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, bac.Id);

            // Add symbols for user 2
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, ba.Id);

            // Add symbols for user 3
            await _portfolioRepo.AddPortfolioSymbol(portfolio3.Id, bac.Id);

            // Setup data
            var marketHours = new NasdaqMarketHours();
            var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, marketHours.EndHoursUTC, marketHours.EndMinutesUTC, 0);

            var jpmQuote = new SingleQuoteData(jpm.Id, "JPM", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(jpm.Id, jpmQuote));

            var baQuote = new SingleQuoteData(ba.Id, "BA", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(ba.Id, baQuote));

            var bacQuote = new SingleQuoteData(bac.Id, "BAC", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(bac.Id, bacQuote));


            var query = new PortfolioStocksUpdateQuery(AppTestSettings.Instance.GetDbOptionsMonitor());

            var symbolIDs = await query.Get(100, endDate);

            Assert.Equal(3, symbolIDs.Count());
            Assert.Contains(jpm.Id, symbolIDs);
            Assert.Contains(ba.Id, symbolIDs);
            Assert.Contains(bac.Id, symbolIDs);
        }

        [Fact]
        public async Task Query_ShouldReturnNoSymbols_WhenStocksAllUpToDate()
        {
            var jpm = await _symbolRepo.GetSymbol("JPM");
            var ba = await _symbolRepo.GetSymbol("BA");
            var bac = await _symbolRepo.GetSymbol("bac");

            var portfolio1 = await CreateUserAndPortfolio("p1");
            var portfolio2 = await CreateUserAndPortfolio("p2");
            var portfolio3 = await CreateUserAndPortfolio("p3");

            // Add symbols for user 1
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, ba.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, bac.Id);

            // Add symbols for user 2
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, ba.Id);

            // Add symbols for user 3
            await _portfolioRepo.AddPortfolioSymbol(portfolio3.Id, bac.Id);

            // Setup data
            var marketHours = new NasdaqMarketHours();
            var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, marketHours.EndHoursUTC, marketHours.EndMinutesUTC, 0);

            var jpmQuote = new SingleQuoteData(jpm.Id, "JPM", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(jpm.Id, jpmQuote));

            var baQuote = new SingleQuoteData(ba.Id, "BA", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(ba.Id, baQuote));

            var bacQuote = new SingleQuoteData(bac.Id, "BAC", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(bac.Id, bacQuote));


            var query = new PortfolioStocksUpdateQuery(AppTestSettings.Instance.GetDbOptionsMonitor());

            var symbolIDs = await query.Get(100, endDate);

            Assert.Equal(3, symbolIDs.Count());
            Assert.Contains(jpm.Id, symbolIDs);
            Assert.Contains(ba.Id, symbolIDs);
            Assert.Contains(bac.Id, symbolIDs);
        }

        [Fact]
        public async Task Query_ShouldReturnStaleSymbols()
        {
            var jpm = await _symbolRepo.GetSymbol("JPM");
            var ba = await _symbolRepo.GetSymbol("BA");
            var bac = await _symbolRepo.GetSymbol("bac");

            var portfolio1 = await CreateUserAndPortfolio("p1");
            var portfolio2 = await CreateUserAndPortfolio("p2");

            // Add symbols for user 1
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, ba.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, bac.Id);

            // Add symbols for user 2
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio2.Id, ba.Id);

            // Setup data
            var marketHours = new NasdaqMarketHours();
            var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, marketHours.EndHoursUTC, marketHours.EndMinutesUTC, 0);

            var jpmQuote = new SingleQuoteData(jpm.Id, "JPM", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(jpm.Id, jpmQuote));

            var baQuote = new SingleQuoteData(ba.Id, "BA", 1, 0, 0, endDate.AddHours(1));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(ba.Id, baQuote));

            var bacQuote = new SingleQuoteData(bac.Id, "BAC", 1, 0, 0, endDate);
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(bac.Id, bacQuote));

            var query = new PortfolioStocksUpdateQuery(AppTestSettings.Instance.GetDbOptionsMonitor());

            var symbolIDs = await query.Get(100, endDate);

            Assert.Equal(2, symbolIDs.Count());
            Assert.Contains(jpm.Id, symbolIDs);
            Assert.Contains(bac.Id, symbolIDs);
        }

        [Fact]
        public async Task Query_ShouldReturnSymbosl_ByLastUpdatedFirst()
        {
            var jpm = await _symbolRepo.GetSymbol("JPM");
            var ba = await _symbolRepo.GetSymbol("BA");
            var bac = await _symbolRepo.GetSymbol("bac");

            var portfolio1 = await CreateUserAndPortfolio("p1");

            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, jpm.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, ba.Id);
            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, bac.Id);

            // Setup data
            var marketHours = new NasdaqMarketHours();
            var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, marketHours.EndHoursUTC, marketHours.EndMinutesUTC, 0);

            var jpmQuote = new SingleQuoteData(jpm.Id, "JPM", 1, 0, 0, endDate.AddHours(-1));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(jpm.Id, jpmQuote));

            var baQuote = new SingleQuoteData(ba.Id, "BA", 1, 0, 0, endDate.AddHours(-5));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(ba.Id, baQuote));

            var bacQuote = new SingleQuoteData(bac.Id, "BAC", 1, 0, 0, endDate.AddHours(-4));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(bac.Id, bacQuote));

            var query = new PortfolioStocksUpdateQuery(AppTestSettings.Instance.GetDbOptionsMonitor());

            var symbolIDs = await query.Get(100, endDate);

            Assert.Equal(3, symbolIDs.Count());
            Assert.Equal(baQuote.SymbolId, symbolIDs.ElementAt(0));
            Assert.Equal(bacQuote.SymbolId, symbolIDs.ElementAt(1));
            Assert.Equal(jpmQuote.SymbolId, symbolIDs.ElementAt(2));
        }

        [Fact]
        public async Task Query_ShouldLimitSymbolsByTop()
        {
            var jpm = await _symbolRepo.GetSymbol("JPM");
            var ba = await _symbolRepo.GetSymbol("BA");
            var bac = await _symbolRepo.GetSymbol("bac");

            var portfolio1 = await CreateUserAndPortfolio("p1");

            await _portfolioRepo.AddPortfolioSymbol(portfolio1.Id, jpm.Id);

            // Setup data
            var marketHours = new NasdaqMarketHours();
            var endDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, marketHours.EndHoursUTC, marketHours.EndMinutesUTC, 0);

            var jpmQuote = new SingleQuoteData(jpm.Id, "JPM", 1, 0, 0, endDate.AddHours(-1));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(jpm.Id, jpmQuote));

            var baQuote = new SingleQuoteData(ba.Id, "BA", 1, 0, 0, endDate.AddHours(-5));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(ba.Id, baQuote));

            var bacQuote = new SingleQuoteData(bac.Id, "BAC", 1, 0, 0, endDate.AddHours(-4));
            await _stockRepo.UpsertSingleQuoteData(new FomoAPI.Infrastructure.Stocks.UpsertSingleQuoteData(bac.Id, bacQuote));

            var query = new PortfolioStocksUpdateQuery(AppTestSettings.Instance.GetDbOptionsMonitor());

            var symbolIDs = await query.Get(1, endDate);

            Assert.Single(symbolIDs);
        }

        private async Task<Portfolio> CreateUserAndPortfolio(string portfolioName)
        {
            var userId = await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString);

            return await _portfolioRepo.CreatePortfolio(userId, portfolioName);
        }

    }
}
