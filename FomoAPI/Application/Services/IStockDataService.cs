using FomoAPI.Application.DTOs;
using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <summary>
    /// Service for handling stock data
    /// </summary>
    public interface IStockDataService
    {
        /// <summary>
        /// Retrieves the single quote data for the given symbols and subrscribes to them.
        /// </summary>
        /// <param name="symbolIds">Ids of the symbols to get single quote data from.</param>
        /// <returns>The stock data <see cref="IEnumerable{StockSingleQuoteDataDTO}"/> for the given symbol ids. </returns>
        Task<IEnumerable<StockSingleQuoteDataDTO>> SubcribeSingleQuoteData(IEnumerable<int> symbolIds);

        /// <summary>
        /// Retrieves the single quote data for the given symbols
        /// </summary>
        /// <param name="symbolId">Id for the symbol.</param>
        /// <returns>The stock data <see cref="StockSingleQuoteDataDTO"/>.</returns>
        Task<StockSingleQuoteDataDTO> GetSingleQuoteData(int symbolId);

        /// <summary>
        /// Saves the single quote query result or updates it if it already exists.
        /// </summary>
        /// <param name="query"><see cref="SingleQuoteQuery"/> to upsert data for.</param>
        /// <param name="queryResult"><see cref="SingleQuoteQueryResult"/> stock data results to save.</param>
        public Task UpsertSingleQuoteData(SingleQuoteQuery query, SingleQuoteQueryResult queryResult);

        /// <summary>
        /// Adds a subscriber to the subscription of stock query to queued up and executed.
        /// </summary>
        /// <param name="symbolId">SymbolId to add subscriber for.</param>
        /// <remarks>Adds query to <see cref="QuerySubscriptions"/></remarks>
        void AddSubscriberToSingleQuote(int symbolId);
    }
}
