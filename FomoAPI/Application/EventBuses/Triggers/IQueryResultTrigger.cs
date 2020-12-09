using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.Triggers
{
    /// <summary>
    /// A trigger command to be executed after a query.
    /// </summary>
    public interface IQueryResultTrigger
    {
        /// <summary>
        /// Execute the Trigger.
        /// </summary>
        /// <param name="result">The result of the query.</param>
        Task Execute(StockQueryResult result);
    }
}
