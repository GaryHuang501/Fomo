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
        /// <returns>IEnumerable<int> containing the portfolio ids.</returns>
        Task<IEnumerable<int>> GetPortfolioIds(Guid userId);


        /// <summary>
        /// Delete symbol from portfolio
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to delete symbol from</param>
        /// <param name="symbolId">Id of symbol to delete</param>
        Task DeletePortfolioSymbol(int portfolioSymbolID);

        /// <summary>
        /// Rename portfolio to given name
        /// </summary>
        /// <param name="portfolioId">Id of portfolio to rename</param>
        /// <param name="newName">New name for the portfolio</param>
        /// <returns>If portfolio was successfully updated</returns>
        Task<bool> RenamePortfolio(int portfolioId, string newName);

        /// <summary>
        /// Update portfolio symbol average price
        /// </summary>
        /// <param name="portfolioSymbolId">Id of portfolio symbol to update.</param>
        /// <param name="averagePrice">New average price value.</param>
        /// <returns>If the given portfolio symbol was updated.</returns>
        Task<bool> UpdateAveragePrice(int portfolSymbolId, decimal averagePrice);

        /// <summary>
        /// Reorder the given portfolio symbol and update  ort order of other PortfolioSymbol in portfolio
        /// to match new ordering.
        /// </summary>
        /// <param name="portfolioId">Id of portfolio</param>
        /// <param name="portfolioSymbolId">Id of reordered portfolioSymbol</param>
        /// <param name="newSortOrder">New sort order</param>
        /// <returns>True if rows updated. Otherwise portfolio symbol was not found.</returns>
        Task<bool> ReorderPortfolioSymbol(int portfolioId, IDictionary<int, int> portfolioSymbolIdToSortOrder);
    }
}