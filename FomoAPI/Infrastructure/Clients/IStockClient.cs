using FomoAPI.Domain.Stocks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <summary>
    /// Client to query stock data from a third party provider.
    /// </summary>
    public interface IStockClient
    {
        /// <summary>
        /// Execute Query for Single Quote (Global Quote) Data
        /// </summary>
        /// <param name="query">Query Object for Single Quote Data</param>
        /// <returns></returns>
        Task<AlphaVantageQueryResult<StockSingleQuoteData>> GetSingleQuoteData(AlphaVantageSingleQuoteQuery query);

        /// <summary>
        /// Gets the top ticker search results for a given keyword
        /// </summary>
        /// <param name="keywords">keyword to search for</param>
        /// <returns>IEnumerable of SymbolSearchResult for the top matching symbols.</returns>
        public Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords);
    }
}