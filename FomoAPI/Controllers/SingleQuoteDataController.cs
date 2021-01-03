using System.Collections.Generic;
using System.Threading.Tasks;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingleQuoteDataController : ControllerBase
    {
        private ILogger<SymbolsController> _logger;

        private IStockDataService _stockDataService;

        public SingleQuoteDataController(
                                 IStockDataService stockDataService,
                                 ILogger<SymbolsController> logger)
        {
            _stockDataService = stockDataService;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<StockSingleQuoteDataDTO>>> GetSingleQuoteData([FromQuery] int[] symbolIds)
        {
            List<StockSingleQuoteDataDTO> dataset = new List<StockSingleQuoteDataDTO>();

            foreach (int id in symbolIds)
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