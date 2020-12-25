using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Concurrent;

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

        protected object _addLock = new object();

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

        /// <summary>
        /// Inserts new item into cache if it does not exist. Otherwise update it.
        /// </summary>
        /// <param name="key">The key of the item to add.</param>
        /// <param name="result">The item to cache.</param>
        public virtual void Upsert(TKey key, TResult result)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(_cacheItemSize)
                .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpiryTimeMinutes));

            _cache.Set(key, result, cacheEntryOptions);
        }

        /// <summary>
        /// Adds the entry if it does not exist.
        /// </summary>
        /// <param name="key">The key of the item to add.</param>
        /// <param name="result">The item to cache.</param>
        public void Add(TKey key, TResult result)
        {
            // GetOrCreate is not thread-safe in the sense that the entry
            // factory delegate will only be executed once.
            // TODO: To improve performance we would want to lock by entry rather than  
            // each add.

            lock (_addLock)
            {
                _cache.GetOrCreate(key, entry =>
                {
                    entry.SetSize(_cacheItemSize)
                         .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheExpiryTimeMinutes));

                    return result;
                });
            }
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
