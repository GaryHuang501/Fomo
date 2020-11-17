using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.Services;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Enums;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
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
        public SymbolsController(ISymbolRepository symbolRepository, ISymbolSearchService searchService, ILogger<SymbolsController> logger)
        {
            _symbolRepository = symbolRepository;
            _searchService = searchService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetSearchResults([FromQuery] string keywords, int limit)
        {
            if(string.IsNullOrWhiteSpace(keywords))
            {
                return NotFound("Search result is empty");
            }

            IEnumerable<SymbolSearchResult> matches = await _searchService.GetSearchedTickers(keywords, limit);

            var descendingMatches = matches.OrderByDescending(m => m.Match).ThenBy(m => m.Symbol);

            return Ok(descendingMatches);
        }
    }
}