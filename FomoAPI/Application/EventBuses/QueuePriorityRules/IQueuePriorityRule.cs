using FomoAPI.Domain.Stocks.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    /// <summary>
    /// Rule for determining the priority of the next querys to queue up.
    /// </summary>
    public interface IQueuePriorityRule
    {
        /// <summary>
        /// Get next set of queries needing to be updated.
        /// </summary>
        /// <returns>Prioritized Queries as <see cref="IEnumerable{T}"/> of type <see cref="StockQuery"/></returns>
        Task<IEnumerable<StockQuery>> GetPrioritizedQueries();

        /// <summary>
        /// Reset the priority for the query.
        /// </summary>
        /// <param name="query"><see cref="StockQuery"/> to reset priority for.</param>
        void ResetPriority(StockQuery query);
    }
}
