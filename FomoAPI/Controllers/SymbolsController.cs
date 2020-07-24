using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public SymbolsController(ISymbolRepository symbolRepository, ILogger<SymbolsController> logger)
        {
            _symbolRepository = symbolRepository;
            _logger = logger;
        }

        [HttpGet()]
        public async Task<IActionResult> GetAsync([FromQuery] string ticker, [FromQuery] int exchangeId)
        {
            Symbol symbol = await _symbolRepository.GetSymbol(ticker, ExchangeType.ToExchange(exchangeId));

            if(symbol == null)
            {
                NotFound();
            }

            return Ok(symbol);
        }
    }
}