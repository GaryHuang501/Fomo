using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.Queries
{
    /// <summary>
    /// Query for getting stock data for symbol that exist in a user portfolio and need to be updated.
    /// </summary>
    public interface IPortfolioStocksUpdateQuery
    {
        /// <summary>
        /// Gets the symbol ids for update.
        /// </summary>
        /// <param name="top">Number of symbol ids to return.</param>
        /// <param name="maxDate">Only look for symbols with update date less than this date.</param>
        /// <returns>Ids of symbols that require updates.</returns>
        Task<IEnumerable<int>> Get(int top, DateTime maxDate);
    }
}