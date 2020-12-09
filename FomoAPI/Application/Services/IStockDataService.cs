using FomoAPI.Application.DTOs;
using FomoAPI.Application.EventBuses;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <summary>
    /// Service for handling stock data
    /// </summary>
    public interface IStockDataService
    {
        /// <summary>
        /// Retrieves the single quote DTO containing fetch result and data for a stock
        /// </summary>
        /// <param name="ticker">Ticker for the stock.</param>
        /// <returns>The stock data <see cref="StockSingleQuoteDataDTO"/>.</returns>
        Task<StockSingleQuoteDataDTO> GetSingleQuoteData(int symbolId);

        /// <summary>
        /// Adds a subscriber to the subscription of stock query to queued up and executed.
        /// </summary>
        /// <param name="symbolId">SymbolId to add subscriber for.</param>
        /// <remarks>Adds query to <see cref="QuerySubscriptions"/></remarks>
        void AddSubscriberToSingleQuote(int symbolId);
    }
}
