using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.Triggers
{
    public interface IQueryResultTrigger
    {
        Task ExecuteAsync(ISubscriptionQueryResult result);
    }
}
