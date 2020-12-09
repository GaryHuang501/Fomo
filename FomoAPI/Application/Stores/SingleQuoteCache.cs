using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using Microsoft.Extensions.Options;

namespace FomoAPI.Application.Stores
{
    public class SingleQuoteCache : ResultCache<int, SingleQuoteQueryResult>
    {
        public const string CacheName = "SingleQuoteCache";

        public SingleQuoteCache(IOptionsMonitor<CacheOptions> optionsAccessor)
            : base(optionsAccessor.Get(CacheName))
        {
        }
    }
}
