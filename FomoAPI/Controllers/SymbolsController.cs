using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SymbolsController : ControllerBase
    {

        private ILogger<SymbolsController> _logger;

        private ISymbolSearchService _searchService;

        public SymbolsController(
                                 ISymbolSearchService searchService, 
                                 ILogger<SymbolsController> logger)
        {
            _searchService = searchService;
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

            var descendingMatches = matches.OrderByDescending(m => m.Match).ThenBy(m => m.Ticker);

            return Ok(descendingMatches);
        }
    }
}