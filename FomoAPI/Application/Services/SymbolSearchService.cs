using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <inheritdoc cref="ISymbolSearchService"/>
    public class SymbolSearchService : ISymbolSearchService
    {
        private readonly IStockClient _client;
        private readonly ResultCache<string, IEnumerable<SymbolSearchResult>> _cache;

        public SymbolSearchService(IStockClient client, ResultCache<string, IEnumerable<SymbolSearchResult>> cache)
        {
            _client = client;
            _cache = cache;
        }
        public async Task<IEnumerable<SymbolSearchResult>> GetSearchedTickers(string keywords)
        {
            if (!_cache.TryGet(keywords, out IEnumerable<SymbolSearchResult> result))
            {
               result = await _client.GetSearchedTickers(keywords);
                _cache.Add(keywords, result);
            }

            return result;
        }
    }
}
