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

        private readonly IStockDataRepository _stockDataRepository;

        private readonly ILogger<SingleQuoteContext> _logger;

        private SingleQuoteQuery _query;

        private SingleQuoteQueryResult _queryResult;

        public SingleQuoteContext(
            IStockClient stockClient,
            SingleQuoteCache queryResultCache,
            ISymbolRepository symbolRepository,
            IStockDataRepository stockDataRepository,
            SingleQuoteQuery query, 
            ILogger<SingleQuoteContext> logger)
        {
            _stockClient = stockClient;
            _queryResultCache = queryResultCache;
            _symbolRepository = symbolRepository;
            _stockDataRepository = stockDataRepository;
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

            _queryResultCache.Upsert(_query.SymbolId, _queryResult);
            _logger.LogTrace("Query SymbolId {id} added to cache", _query.SymbolId);

            const int primaryKeyViolationCode = 2627;

            try
            {
                bool successfulSave = await _stockDataRepository.UpsertSingleQuoteData(new UpsertSingleQuoteData(_query.SymbolId, _queryResult.Data));
                _logger.LogTrace("Query SymbolId {id} saved to database", _query.SymbolId);

                if (!successfulSave)
                {
                    throw new ArgumentException($"query for symbolId {_query.SymbolId} was not able to be updated");
                }
            }
            catch (SqlException ex) when(ex.ErrorCode == primaryKeyViolationCode)
            {
                _logger.LogError("Query SymbolId {id} was duplicate insert in database", _query.SymbolId);

                // Ignore the error since it means a race condition where we tried insert twice.
                // But that's fine since the stock data is unlikely to be different if it's that close in time.
                // This is more performant than adding locks as this case should be very exceptional.
            }
        }

        /// <summary>
        /// Gets the query result from the cache and then database if it doesn't exist.
        /// </summary>
        /// <returns>The <see cref="SingleQuoteQueryResult"/>. If no query result exists, null is returned.</returns>
        public Task<StockQueryResult> GetQueryResult(int symbolId)
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

