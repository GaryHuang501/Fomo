using FomoAPI.Domain.Stocks;
using System;
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
        Task AddPortfolioSymbol(int portfolioId, int symbolId);

        Task AddPriceAlert(Guid userId);

        Task<Portfolio> CreatePortfolio(Guid userId, string name);
        Task DeletePortfolio(int portfolioId);

        Task<Portfolio> GetPortfolio(int portfolioId);

        Task RemovePortfolioSymbol(int portfolioId, int symbolId);
    }
}