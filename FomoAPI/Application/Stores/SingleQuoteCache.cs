using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using Microsoft.Extensions.Options;

namespace FomoAPI.Application.Stores
{
    public class SingleQuoteCache : ResultCache<ISubscribableQuery, ISubscriptionQueryResult>, IQueryCache
    {
        public const string CacheName = "SingleQuoteCache";

        public SingleQuoteCache(IOptionsMonitor<CacheOptions> optionsAccessor)
            : base(optionsAccessor.Get(CacheName))
        {
        }
    }
}
