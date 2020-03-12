using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    /// Abstract class for caching query results. 
    /// </summary>
    public abstract class AbstractQueryResultCache : IQueryCache
    {
        protected readonly MemoryCache _cache;

        protected readonly long _cacheItemSize;

        protected readonly int _cacheExpiryTimeMinutes;


        public AbstractQueryResultCache(IQueryCacheOptions options)
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = options.CacheSize,           
            });

            _cacheItemSize = options.CacheItemSize;
            _cacheExpiryTimeMinutes = options.CacheExpiryTimeMinutes;
        }

        public virtual void Add(ISubscribableQuery query, ISubscriptionQueryResult result)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(_cacheItemSize)
                .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpiryTimeMinutes));

            _cache.Set(query, result, cacheEntryOptions);
        }

        public virtual bool TryGet(ISubscribableQuery query, out ISubscriptionQueryResult result)
        {
            return _cache.TryGetValue(query, out result);
        }

        public virtual void RemoveQuery(ISubscribableQuery query)
        {
            _cache.Remove(query);
        }
    }
}
