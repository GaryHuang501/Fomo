using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class SingleQuoteCacheOptions: IQueryCacheOptions
    {
        public long CacheSize { get; set; }

        public long CacheItemSize { get; set; }

        public int CacheExpiryTimeMinutes { get; set; }
    }
}
