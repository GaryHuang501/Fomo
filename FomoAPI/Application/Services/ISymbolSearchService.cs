using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <summary>
    /// Service for 
    /// </summary>
    public interface ISymbolSearchService
    {
        /// <summary>
        /// Gets the top ticker search results for a given keyword
        /// </summary>
        /// <param name="keywords">keyword to search for</param>
        /// <param name="limit">How many results to return</param>
        /// <returns>IEnumerable of SymbolSearchResult for the top matching symbols.</returns>
        Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords, int limit);
    }
}
