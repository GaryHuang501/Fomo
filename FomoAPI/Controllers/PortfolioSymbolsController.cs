using System;
using System.Threading.Tasks;
using FomoAPI.Application.Commands.Portfolio;
using FomoAPI.Application.Patches;
using FomoAPI.Controllers.Authorization;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Authorize]
    [ApiController]
    public class PortfolioSymbolsController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly PortfolioSymbolValidator _validator;
        private readonly ILogger<PortfolioSymbolsController> _logger;

        public PortfolioSymbolsController(IPortfolioRepository portfolioRepository, PortfolioSymbolValidator validator, ILogger<PortfolioSymbolsController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _validator = validator;
            _logger = logger;
        }

        [HttpPost("api/portfolios/{portfolioId}/portfolioSymbols")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = PolicyTypes.PortfolioOwner)]
        public async Task<ActionResult<PortfolioSymbol>> AddPortfolioSymbol(int portfolioId, [FromBody] AddPortfolioSymbolCommand addPortfolioSymbolCommand)
        {
            var portfolioSymbol = await _portfolioRepository.AddPortfolioSymbol(portfolioId, addPortfolioSymbolCommand.SymbolId);

            if (portfolioSymbol == null)
            {
                return BadRequest("Symbol does not exist or already exists in portfolio");
            }

            return Ok(portfolioSymbol);
        }


        [HttpPatch("api/portfolios/{portfolioId}/portfolioSymbols/{portfolioSymbolId}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> UpdatePortfolioSymbol(int portfolioId, int portfolioSymbolId, [FromBody] JsonPatchDocument<PortfolioSymbol> patches)
        {
            var portfolioSymbol = await _portfolioRepository.GetPortfolioSymbol(portfolioSymbolId);

            if (portfolioSymbol == null)
            {
                return NotFound("Portfolio does not exist");
            }

            var updatedPortfolioSymbol = patches.CopyTo(portfolioSymbol);

            var validationResult = await _validator.ValidateAsync(updatedPortfolioSymbol);

            if (!validationResult.IsValid)
            {
                return BadRequest($"Update patch is invalid: {validationResult}");
            }

            var success = await _portfolioRepository.UpdatePortfolioSymbol(updatedPortfolioSymbol);

            if (!success)
            {
                return NotFound("PortfolioSymbol was not updated");
            }

            return Ok();
        }

        [HttpPatch("api/portfolios/{portfolioId}/portfolioSymbols/sortOrder")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> ReorderPortfolioSymbol(int portfolioId, [FromBody] ReorderPortfolioCommand reorderPortfolioCommand)
        {
            var success = await _portfolioRepository.ReorderPortfolioSymbol(portfolioId, reorderPortfolioCommand.PortfolioSymbolIdToSortOrder);

            if (!success)
            {
                return NotFound("Portfolio or PortfolioSymbol was not found.");
            }
            return Ok();
        }

        [HttpDelete("api/portfolios/{portfolioId}/portfolioSymbols/{portfolioSymbolid}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> DeletePortfolioSymbol(int portfolioId, int portfolioSymbolid)
        {
            await _portfolioRepository.DeletePortfolioSymbol(portfolioSymbolid);

            return Ok();
        }
    }
}
