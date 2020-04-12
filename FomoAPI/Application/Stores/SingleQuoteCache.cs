using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Options;

namespace FomoAPI.Application.Stores
{
    public class SingleQuoteCache : AbstractQueryResultCache
    {
        public const string CacheName = "SingleQuoteCache";
        public SingleQuoteCache(IOptionsMonitor<CacheOptions> optionsAccessor)
            : base(optionsAccessor.Get(CacheName))
        {
        }
    }
}
