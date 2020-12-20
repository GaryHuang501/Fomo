using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.DTOs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

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
