using FomoAPI.Domain.Stocks;
using FomoAPI.Application.Stores;
using System;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using System.Threading.Tasks;
using FomoAPI.Domain.Stocks.Queries;
using FomoAPI.Infrastructure.Stocks;
using Microsoft.Data.SqlClient;

namespace FomoAPI.Application.Services
{
    /// <inheritdoc <see cref="IStockDataService"/>/>
    public class StockDataService : IStockDataService
    {
        private readonly IStockDataRepository _stockDataRepository;

        private readonly SingleQuoteCache _singleQuoteCache;
        private readonly QuerySubscriptions _querySubscriptions;

        public StockDataService(IStockDataRepository stockDataRepository, 
                                SingleQuoteCache singleQuoteCache,
                                QuerySubscriptions querySubscriptions)
        {
            _stockDataRepository = stockDataRepository;
            _singleQuoteCache = singleQuoteCache;
            _querySubscriptions = querySubscriptions;
        }

        /// <summary>
        /// Gets the single quote stock data for a ticker.
        /// Retrieves from the cache <see cref="SingleQuoteCache"/> if it exists,
        /// otherwise it will fetch from the database.
        /// 
        /// If does not exist in the database it will return no data.
        /// Returns empty 
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns>The <see cref="StockSingleQuoteDataDTO"/></returns>
        public async Task<StockSingleQuoteDataDTO> GetSingleQuoteData(int symbolId)
        {
            SingleQuoteQueryResult queryResult;
            StockSingleQuoteData data;

            if (_singleQuoteCache.TryGet(symbolId, out queryResult))
            {
                return new StockSingleQuoteDataDTO(symbolId, queryResult.Data);
            }

            data = await _stockDataRepository.GetSingleQuoteData(symbolId);


            if(data != null)
            {
                _singleQuoteCache.Upsert(symbolId, queryResult);
                return new StockSingleQuoteDataDTO(symbolId, data);
            }

            return StockSingleQuoteDataDTO.CreateNoDataExists(symbolId);
        }

        public void AddSubscriberToSingleQuote(int symbolId)
        {
            var query = new SingleQuoteQuery(symbolId);
            _querySubscriptions.AddSubscriber(query);
        }
    }
}
