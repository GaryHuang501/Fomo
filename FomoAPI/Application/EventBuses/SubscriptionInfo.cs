using FomoAPI.Domain.Stocks.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    public class SubscriptionInfo
    {
        public StockQuery Query { get; private set; }

        public long SubscriberCount { get; private set; }

        public SubscriptionInfo(StockQuery query, long subscriberCount)
        {
            Query = query;
            SubscriberCount = subscriberCount;
        }

        public bool HasSubscribers() => SubscriberCount > 0;
    }
}
