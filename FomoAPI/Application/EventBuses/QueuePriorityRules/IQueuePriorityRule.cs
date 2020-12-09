using FomoAPI.Domain.Stocks.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    public interface IQueuePriorityRule
    {
        Task<IEnumerable<StockQuery>> Sort(QuerySubscriptions querySubscriptions);   
    }
}
