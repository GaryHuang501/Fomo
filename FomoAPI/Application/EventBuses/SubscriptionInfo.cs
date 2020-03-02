using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    public class SubscriptionInfo
    {
        public ISubscribableQuery Query { get; private set; }

        public long SubscriberCount { get; private set; }

        public SubscriptionInfo(ISubscribableQuery query, long subscriberCount)
        {
            Query = query;
            SubscriberCount = subscriberCount;
        }
    }
}
