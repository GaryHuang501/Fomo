using System;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class CacheOptions
    {
        [Range(1, long.MaxValue)]
        public long CacheSize { get; set; }

        [Range(1, long.MaxValue)]
        public long CacheItemSize { get; set; }

        [Range(1, long.MaxValue)]
        public int CacheExpiryTimeMinutes { get; set; }
    }
}
