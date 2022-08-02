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
        /// <returns><see cref="IEnumerable{Symbol}"/> for the matching tickers</returns>
        Task<IEnumerable<Symbol>> GetSymbols(IEnumerable<string> tickers);

        /// <summary>
        /// Get the symbol that match the given tickers
        /// </summary>
        /// <param name="ticker">Ticker to search for.</param>
        /// <returns>Matching <see cref="Symbol"/> for the given ticker. Otherwise null.</returns>
        Task<Symbol> GetSymbol(string ticker);

        /// <summary>
        /// Get a list of symbols that match the given symbol ids
        /// </summary>
        /// <param name="symbolIds">SymbolIds to search for.</param>
        /// <returns><see cref="IEnumerable{Symbol}"/> of the matching SymbolIds.
        /// Empty IEnumerable will be returned if no results match."/></returns>
        Task<IEnumerable<Symbol>> GetSymbols(IEnumerable<int> symbolIds);

        /// <summary>
        /// Get the symbol that match the given symbol id.
        /// </summary>
        /// <param name="symbolId">SymbolId to search for.</param>
        /// <returns>Matching <see cref="Symbol"/> for the given SymbolId. Otherwise null.</returns>
        Task<Symbol> GetSymbol(int symbolId);
    }
}
