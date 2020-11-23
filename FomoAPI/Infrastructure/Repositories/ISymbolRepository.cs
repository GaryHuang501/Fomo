using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for CRUD operations on the stock symbol/ticker info for the database.
    /// </summary>
    public interface ISymbolRepository
    {
        /// <summary>
        /// Get a list of symbols that match the given tickers
        /// </summary>
        /// <param name="tickers">Tickers to search for.</param>
        /// <returns><see cref="IEnumerable"/> of <see cref="Symbol"/> for the given tickers</returns>
        Task<IEnumerable<Symbol>> GetSymbols(IEnumerable<string> tickers);

        /// <summary>
        /// Get the symbol that match the given tickers
        /// </summary>
        /// <param name="ticker">Ticker to search for.</param>
        /// <returns>Matching <see cref="Symbol"/> for the given ticker. Otherwise null.</returns>
        Task<Symbol> GetSymbol(string tickers);
    }
}
