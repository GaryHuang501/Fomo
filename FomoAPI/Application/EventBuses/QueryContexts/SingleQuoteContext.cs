using FomoAPI.Application.Stores;
using FomoAPI.Application.EventBuses.Triggers;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using System.Collections.Generic;
using System.Threading.Tasks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Repositories;
using System;
using Microsoft.Data.SqlClient;
using FomoAPI.Infrastructure.Stocks;
using FomoAPI.Application.Services;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Application.EventBuses.QueryContexts
{
    /// <summary>
    /// Context to control where the single quote query results will be fetched from, saved, and any post triggers
    /// </summary>
    public class SingleQuoteContext : IQueryContext
    {
        private readonly SingleQuoteCache _queryResultCache;

        private readonly ISymbolRepository _symbolRepository;

        private readonly IStockClient _stockClient;

        private readonly IStockDataService _stockDataService;

        private readonly ILogger<SingleQuoteContext> _logger;

        private SingleQuoteQuery _query;

        private SingleQuoteQueryResult _queryResult;

        public SingleQuoteContext(
            IStockClient stockClient,
            SingleQuoteCache queryResultCache,
            ISymbolRepository symbolRepository,
            IStockDataService stockDataService,
            SingleQuoteQuery query, 
            ILogger<SingleQuoteContext> logger)
        {
            _stockClient = stockClient;
            _queryResultCache = queryResultCache;
            _symbolRepository = symbolRepository;
            _stockDataService = stockDataService;
            _query = query;
            _logger = logger;
        }

        /// <summary>
        /// Executes the query and saves the single quote data to the <see cref="SingleQuoteCache"/> and the database.
        /// </summary>
        public async Task SaveQueryResultToStore()
        {
            Symbol symbol = await _symbolRepository.GetSymbol(_query.SymbolId);
            _queryResult = await _stockClient.GetSingleQuoteData(symbol.Ticker, symbol.ExchangeName);
            await _stockDataService.UpsertSingleQuoteData(_query, _queryResult);
        }

        /// <summary>
        /// Gets the query result from the cache and then database if it doesn't exist.
        /// </summary>
        /// <returns>The <see cref="SingleQuoteQueryResult"/>. If no query result exists, null is returned.</returns>
        public Task<StockQueryResult> GetCachedQueryResult(int symbolId)
        {
            if (_queryResultCache.TryGet(symbolId, out SingleQuoteQueryResult result))
            {
                return Task.FromResult<StockQueryResult>(result);
            }

            return Task.FromResult<StockQueryResult>(null);
        }

        public Task ExecuteResultTriggers()
        {
            return Task.CompletedTask;
        }
    }
}

