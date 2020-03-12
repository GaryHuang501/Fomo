using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace FomoAPI.Application.Stores
{
    public class SingleQuoteCache : AbstractQueryResultCache
    {
        public SingleQuoteCache(IOptionsMonitor<SingleQuoteCacheOptions> optionsAccessor)
            : base(optionsAccessor.CurrentValue)
        {
        }
    }
}
