using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.DTOs;
using FomoAPI.Domain.Stocks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Stores
{
    /// <summary>
    /// Cache for storing stock symbol search results from client.
    /// Keyed by search keyword.
    /// </summary>
    public class StockSearchCache : ResultCache<string, IEnumerable<SymbolSearchResultDTO>>
    {
        public const string CacheName = "StockSearchCache";

        public StockSearchCache(IOptionsMonitor<CacheOptions> optionsAccessor)
            : base(optionsAccessor.Get(CacheName))
        {
        }
    }
}
