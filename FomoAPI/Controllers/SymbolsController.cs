using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    /// <summary>
    /// Manages stock symbol info
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SymbolsController : ControllerBase
    {
        private readonly ILogger<SymbolsController> _logger;

        private readonly ISymbolSearchService _searchService;

        public SymbolsController(
                                 ISymbolSearchService searchService, 
                                 ILogger<SymbolsController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the best matching stock symbols by keywords.
        /// </summary>
        /// <param name="keywords">Keywords to search for.</param>
        /// <param name="limit">Limit of how many matches to return.</param>
        /// <response code="200">Returns collection of matches ordered by best matches first.</response>
         /// <response code="400">Invalid key words.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<SymbolSearchResultDTO>>> GetSearchResults([FromQuery] string keywords, [FromQuery] int limit)
        {
            if(string.IsNullOrWhiteSpace(keywords))
            {
                return BadRequest("Search result is empty");
            }

            IEnumerable<SymbolSearchResultDTO> matches = await _searchService.GetSearchedTickers(keywords, limit);

            var descendingMatches = matches.OrderByDescending(m => m.Match).ThenBy(m => m.Ticker);

            return Ok(descendingMatches);
        }
    }
}