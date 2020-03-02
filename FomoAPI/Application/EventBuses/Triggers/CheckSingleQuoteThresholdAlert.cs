using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses.Triggers
{
    public class CheckSingleQuoteThresholdAlert : IQueryResultTrigger
    {
        public Task ExecuteAsync(ISubscriptionQueryResult result)
        {
            throw new NotImplementedException();
        }
    }
}
