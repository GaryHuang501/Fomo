using FomoAPI.Domain.Stocks;
using System;
using System.Collections;
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
        Task<PortfolioSymbol> AddPortfolioSymbol(int portfolioId, int symbolId);

        Task AddPriceAlert(Guid userId);

        Task<Portfolio> CreatePortfolio(Guid userId, string name);

        Task DeletePortfolio(int portfolioId);

        Task<Portfolio> GetPortfolio(int portfolioId);

        Task DeletePortfolioSymbol(int portfolioSymbolID);

        Task<bool> RenamePortfolio(int portfolioId, string newName);

        Task<bool> ReorderPortfolioSymbol(int portfolioId, IDictionary<string, int> portfolioSymbolIdToSortOrder);
    }
}