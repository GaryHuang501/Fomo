using System.Collections.Generic;
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
        /// Get single quote data for the given symbol ids.
        /// </summary>
        /// <param name="sids">Symbol Ids to get the data for. Short hand notation to reduce query size.</param>
        /// <returns><see cref="IEnumerable{T}"/> of the Single Quote Data for the symbols.</returns>
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