using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.QueuePriorityRules
{
    public interface IQueuePriorityRule
    {
        IEnumerable<ISubscribableQuery> Sort(QuerySubscriptions querySubscriptions);   
    }




}
