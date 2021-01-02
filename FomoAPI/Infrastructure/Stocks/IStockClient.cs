using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks
{
    /// <summary>
    /// Client to query stock data from a third party provider.
    /// </summary>
    public interface IStockClient
    {
        /// <summary>
        /// Get the single quote data from the third party api.
        /// </summary>
        /// <param name="symbol">Symbol for the query be executed.</param>
        /// <returns><see cref="StockSingleQuoteData"/></returns>
        Task<SingleQuoteQueryResult> GetSingleQuoteData(string ticker, string exchangeName);

        /// <summary>
        /// Gets the top ticker search results for a given keyword from the client
        /// </summary>
        /// <param name="keywords">Keyword to search for</param>
        /// <returns>IEnumerable of SymbolSearchResult for the top matching symbols.</returns>
        public Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords);
    }
}