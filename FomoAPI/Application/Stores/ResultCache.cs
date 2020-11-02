using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    /// Abstract wrapper class for caching results through memory cache.
    /// </summary>
    public abstract class ResultCache<TKey, TResult>
    {
        protected readonly MemoryCache _cache;

        protected readonly long _cacheItemSize;

        protected readonly int _cacheExpiryTimeMinutes;

        public ResultCache(CacheOptions options)
        {
            _cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = options.CacheSize,
            });

            _cacheItemSize = options.CacheItemSize;
            _cacheExpiryTimeMinutes = options.CacheExpiryTimeMinutes;
        }

        public void Dispose()
        {
            _cache.Dispose();
        }

        public virtual void Add(TKey key, TResult result)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(_cacheItemSize)
                .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpiryTimeMinutes));

            _cache.Set(key, result, cacheEntryOptions);
        }

        public virtual bool TryGet(TKey key, out TResult result)
        {
            return _cache.TryGetValue(key, out result);
        }

        public virtual void Remove(TKey key)
        {
            _cache.Remove(key);
        }
    }
}
