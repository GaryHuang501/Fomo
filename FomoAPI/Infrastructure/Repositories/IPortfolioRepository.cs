using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository class for CRUD operations on a user's portfolio.
    /// Security layer should be responsible for restricting 
    /// portfolio access to the user.
    /// </summary>
    public interface IPortfolioRepository
    {


        /// <summary>
        /// Add symbol to portfolio
        /// </summary>
        /// <param name="portfolioId">Id of Portfolio to add symbols to</param>
        /// <param name="symbolId">Id of symbol to add</param>
        /// <returns>Newly Added PortfolioSymbol. Return null if symbol was not added: either symbol Id not found or symbol already exists.</returns>
        Task<PortfolioSymbol> AddPortfolioSymbol(int portfolioId, int symbolId);

        /// <summary>
        /// Create new portfolio
        /// </summary>
        /// <param name="userId">UserId for owner of portfolio</param>
        /// <param name="name">Portfolio name</param>
        /// <returns>The new Portfolio</returns>
        Task<Portfolio> CreatePortfolio(Guid userId, string name);


        /// <summary>
        /// Delete portfolio
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to delete</param>
        /// <returns>Task</returns>
        Task DeletePortfolio(int portfolioId);

        /// <summary>
        /// Get the portfolio and it's symbols
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to fetch</param>
        /// <returns>Portfolio. Null if does not exist.</returns>
        Task<Portfolio> GetPortfolio(int portfolioId);


        /// <summary>
        /// Get the ids of the portfolios the user owns.
        /// </summary>
        /// <param name="userId">User Guid ID</param>
        /// <returns><see cref="IEnumerable{Int32}"/> containing the portfolio ids.</returns>
        Task<IEnumerable<int>> GetPortfolioIds(Guid userId);


        /// <summary>
        /// Get the portfolio Symbol.
        /// </summary>
        /// <param name="portfolioSymbolId">Id of portfolio symbol to fetch</param>
        /// <returns>PortfolioSymbol. Null if does not exist.</returns>
        Task<PortfolioSymbol> GetPortfolioSymbol(int portfolioSymbolId);

        /// <summary>
        /// Delete symbol from portfolio
        /// </summary>
        /// <param name="portfolioSymbolID">Id of <see cref="PortfolioSymbol"/> to delete</param>
        Task DeletePortfolioSymbol(int portfolioSymbolID);

        /// <summary>
        /// Updates the portfoliosymbol with the given resource.
        /// </summary>
        /// <param name="portfolioSymbol">New <see cref="PortfolioSymbol"></see> to update to.</param>
        /// <returns>True if update was successful.</returns>
        Task<bool> UpdatePortfolioSymbol(PortfolioSymbol portfolioSymbol);


        /// <summary>
        /// Updates the portfolio with the given resource.
        /// </summary>
        /// <param name="portfolio">New <see cref="Portfolio"></see> to update to.</param>
        /// <returns>True if update was successful.</returns>
        Task<bool> UpdatePortfolio(Portfolio portfolio);

        /// <summary>
        /// Reorder the given portfolio symbol and update  ort order of other PortfolioSymbol in portfolio
        /// to match new ordering.
        /// </summary>
        /// <param name="portfolioId">Id of portfolio</param>
        /// <param name="portfolioSymbolIdToSortOrder"><see cref="IDictionary{Int32, Int32}"/> to map portfolio symbol id to sort order.</param>
        /// <returns>True if rows updated. Otherwise portfolio symbol was not found.</returns>
        Task<bool> ReorderPortfolioSymbol(int portfolioId, IDictionary<int, int> portfolioSymbolIdToSortOrder);
    }
}