using FomoAPI.Application.EventBuses.Triggers;
using FomoAPI.Domain.Stocks.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueryContexts
{
    /// <summary>
    /// Provides context for a given query type, routing the functionality to correct associated classes.
    /// </summary>
    public interface IQueryContext
    {
        /// <summary>
        /// Executes Query and Saves the result to a form of storage for future retrieval.
        /// </summary>
        Task SaveQueryResultToStore();

        /// <summary>
        /// Execute any triggers or actions that should be performed after the query has been fetched.
        /// This should be run after SaveQueryResultToStore when the data is retrieved and saved.
        /// If not it has not run, execution will be ignored.
        /// </summary>
        Task ExecuteResultTriggers();

        /// <summary>
        /// Gets the query result for the given symbol ID from the cache.
        /// </summary>
        /// <returns>The <see cref="StockQueryResult"/>. Returns Null if it doesn't exist in the cache.</returns>
        Task<StockQueryResult> GetCachedQueryResult(int symbolId);
    }
}
