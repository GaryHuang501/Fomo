using FomoAPI.Domain.Stocks;
using FomoAPI.Application.Stores;
using System;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage;
using System.Threading.Tasks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Stocks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Application.Services
{
    /// <inheritdoc cref="IStockDataService"/>
    public class StockDataService : IStockDataService
    {
        private readonly IStockDataRepository _stockDataRepository;

        private readonly SingleQuoteCache _singleQuoteCache;

        private readonly QuerySubscriptions _querySubscriptions;

        private readonly ILogger<StockDataService> _logger;


        public StockDataService(IStockDataRepository stockDataRepository,
                                SingleQuoteCache singleQuoteCache,
                                QuerySubscriptions querySubscriptions, 
                                ILogger<StockDataService> logger)
        {
            _stockDataRepository = stockDataRepository;
            _singleQuoteCache = singleQuoteCache;
            _querySubscriptions = querySubscriptions;
            _logger = logger;
        }

        /// <summary>
        /// Gets the single quote stock data for a ticker.
        /// Retrieves from the cache <see cref="SingleQuoteCache"/> if it exists,
        /// otherwise it will fetch from the database.
        /// 
        /// If does not exist in the database it will return no data.
        /// Returns empty 
        /// </summary>
        /// <param name="symbolId">Id for the symbol.</param>
        /// <returns>The <see cref="StockSingleQuoteDataDTO"/></returns>
        public async Task<StockSingleQuoteDataDTO> GetSingleQuoteData(int symbolId)
        {
            SingleQuoteQueryResult queryResult;
            SingleQuoteData data;

            if (_singleQuoteCache.TryGet(symbolId, out queryResult))
            {
                return new StockSingleQuoteDataDTO(symbolId, queryResult.Data);
            }

            data = await _stockDataRepository.GetSingleQuoteData(symbolId);


            if(data != null)
            {
                _singleQuoteCache.Add(symbolId, new SingleQuoteQueryResult(data.Ticker, data));
                return new StockSingleQuoteDataDTO(symbolId, data);
            }

            return StockSingleQuoteDataDTO.CreateNoDataExists(symbolId);
        }

        /// <summary>
        /// Saves the single quote query result or updates it if it already exists.
        /// </summary>
        /// <param name="query"><see cref="SingleQuoteQuery"/> to upsert data for.</param>
        /// <param name="queryResult"><see cref="SingleQuoteQueryResult"/> stock data results to save.</param>
        /// <remarks>
        /// Will update the database and then <see cref="SingleQuoteCache"/> if it was successful. 
        /// Error results will be ignored.
        /// </remarks>
        public async Task UpsertSingleQuoteData(SingleQuoteQuery query, SingleQuoteQueryResult queryResult)
        {
            if (queryResult.HasError)
            {
                return;
            }

            _logger.LogTrace("Query SymbolId {id} added to cache", query.SymbolId);

            const int primaryKeyViolationCode = 2627;

            try
            {
                bool successfulSave = await _stockDataRepository.UpsertSingleQuoteData(new UpsertSingleQuoteData(query.SymbolId, queryResult.Data));
                _logger.LogTrace("Query SymbolId {id} saved to database",query.SymbolId);

                if (!successfulSave)
                {
                    throw new ArgumentException($"query for symbolId {query.SymbolId} was not able to be updated");
                }
            }
            catch (SqlException ex) when (ex.ErrorCode == primaryKeyViolationCode)
            {
                _logger.LogError("Query SymbolId {id} was duplicate insert in database", query.SymbolId);

                // Ignore the error since it means a race condition where we tried insert twice.
                // But that's fine since the stock data is unlikely to be different if it's that close in time.
                // This is more performant than adding locks as this case should be very exceptional.
            }

            // Only insert into cache if it successfully saved into database.
            _singleQuoteCache.Upsert(query.SymbolId, queryResult);
        }

        public void AddSubscriberToSingleQuote(int symbolId)
        {
            var query = new SingleQuoteQuery(symbolId);
            _querySubscriptions.AddSubscriber(query);
        }
    }
}
