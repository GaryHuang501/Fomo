using System.Collections.Generic;
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
    /// Manages the single quote data for stocks.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SingleQuoteDataController : ControllerBase
    {
        private readonly ILogger<SymbolsController> _logger;

        private readonly IStockDataService _stockDataService;

        public SingleQuoteDataController(
                                 IStockDataService stockDataService,
                                 ILogger<SymbolsController> logger)
        {
            _stockDataService = stockDataService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the single quote data for the given symbol ids.
        /// </summary>
        /// <param name="sids">Ids of symbols to fetch data for.</param>
        /// <response code="200">Returns collection of single quote data.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Produces("application/json")]
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<StockSingleQuoteDataDTO>>> GetSingleQuoteData([FromQuery] int[] sids)
        {
            List<StockSingleQuoteDataDTO> dataset = new List<StockSingleQuoteDataDTO>();

            foreach (int id in sids)
            {
                // Fetching one stock at a time since values are cached.
                StockSingleQuoteDataDTO quote = await _stockDataService.GetSingleQuoteData(id);
                dataset.Add(quote);

                _stockDataService.AddSubscriberToSingleQuote(id);
            }

            return Ok(dataset);
        }
    }
}