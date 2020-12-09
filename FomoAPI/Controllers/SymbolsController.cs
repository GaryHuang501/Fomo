using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SymbolsController : ControllerBase
    {
        private ISymbolRepository _symbolRepository;

        private ILogger<SymbolsController> _logger;

        private ISymbolSearchService _searchService;

        private IStockDataService _stockDataService;
        public SymbolsController(ISymbolRepository symbolRepository,
                                 ISymbolSearchService searchService, 
                                 IStockDataService stockDataService,
                                 ILogger<SymbolsController> logger)
        {
            _symbolRepository = symbolRepository;
            _searchService = searchService;
            _stockDataService = stockDataService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<SymbolSearchResultDTO>>> GetSearchResults([FromQuery] string keywords, [FromQuery] int limit)
        {
            if(string.IsNullOrWhiteSpace(keywords))
            {
                return NotFound("Search result is empty");
            }

            IEnumerable<SymbolSearchResultDTO> matches = await _searchService.GetSearchedTickers(keywords, limit);

            var descendingMatches = matches.OrderByDescending(m => m.Match).ThenBy(m => m.Symbol);

            return Ok(descendingMatches);
        }

        [HttpGet("singleQuoteData")]
        public async Task<ActionResult<IEnumerable<StockSingleQuoteDataDTO>>> GetSingleQuoteDatas([FromQuery] int[] symbolIds)
        {
            List<StockSingleQuoteDataDTO> dataset = new List<StockSingleQuoteDataDTO>();

            foreach(int id in symbolIds)
            {
                // Fetching one stock at a time is better than at once due to caching.
                StockSingleQuoteDataDTO quote = await _stockDataService.GetSingleQuoteData(id);
                dataset.Add(quote);

                _stockDataService.AddSubscriberToSingleQuote(id);
            }

            return Ok(dataset);
        }
    }
}