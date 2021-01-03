using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Stock Data Database CRUD
    /// </summary>
    public interface IStockDataRepository
    {
        /// <summary>
        /// Get the single quote data for given symbol Id.
        /// </summary>
        /// <param name="symbolId">SymbolId for single quote data to retrieve.</param>
        /// <returns><see cref="SingleQuoteData"/></returns>
        Task<SingleQuoteData> GetSingleQuoteData(int symbolId);

        /// <summary>
        /// Upsert single quote data to database.
        /// </summary>
        /// <param name="singleQuoteData"><see cref="SingleQuoteData"/> to save.</param>
        /// <returns>Whether or not the data was saved.</returns>
        Task<bool> UpsertSingleQuoteData(UpsertSingleQuoteData singleQuoteData);
    }
}
