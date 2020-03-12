using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public interface IQueryCacheOptions
    {
        long CacheSize { get; set; }

        long CacheItemSize { get; set; }

        int CacheExpiryTimeMinutes { get; set; }
    }
}
