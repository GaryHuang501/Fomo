using FomoAPI.Application.DTOs;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Infrastructure.Stocks;
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
        private readonly ISymbolRepository _symbolRepository;
        private readonly StockSearchCache _cache;

        public SymbolSearchService(IStockClient client, ISymbolRepository symbolRepository, StockSearchCache cache)
        {
            _client = client;
            _symbolRepository = symbolRepository;
            _cache = cache;
        }
        public async Task<IEnumerable<SymbolSearchResultDTO>> GetSearchedTickers(string keywords, int limit)
        {
            if (!_cache.TryGet(keywords, out IEnumerable<SymbolSearchResultDTO> dtos))
            {
                var newDtos = await CreateMatchingSymbolDTOs(keywords);

                dtos = newDtos;
                _cache.Upsert(keywords, dtos);
            }

            return dtos.Take(limit).ToList();
        }

        private async Task<IEnumerable<SymbolSearchResultDTO>> CreateMatchingSymbolDTOs(string keywords)
        {
            // Only return symbols that exist in our database.
            // Also our internal Symbol DB identity is what is used to add the symbol the portfolio.
            var newDtos = new List<SymbolSearchResultDTO>();
            IEnumerable<SymbolSearchResult> newSearchResults = await _client.GetSearchedTickers(keywords);
            IEnumerable<Symbol> symbols = await _symbolRepository.GetSymbols(newSearchResults.Select(r => r.Ticker));

            foreach (var result in newSearchResults)
            {
                Symbol matchingSymbol = symbols.FirstOrDefault(s => s.Ticker == result.Ticker);

                if (matchingSymbol != null)
                {
                    newDtos.Add(new SymbolSearchResultDTO(result, matchingSymbol));
                }
            }

            return newDtos;
        }
    }
}
