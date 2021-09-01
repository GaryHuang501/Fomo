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
    /// <summary>
    /// Manages the portfolio stock symbols for portfolios.
    /// </summary>
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

        /// <summary>
        /// Add a new portfolio stock symbol to a portfolio.
        /// </summary>
        /// <param name="portfolioId">Id of portfolio.</param>
        /// <param name="addPortfolioSymbolCommand">New portfolio symbol data.</param>
        /// <response code="200">Portfolio stock symbol successfully added.</response>
        /// <response code="400">Invalid symbol or duplicate portfolio symbol.</response>
        [HttpPost("api/portfolios/{portfolioId}/portfolioSymbols")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
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

        /// <summary>
        /// Update a portfolio stock symbol in a portfolio
        /// </summary>
        /// <param name="portfolioId">Id of the portfolio that the stock symbol belongs to.</param>
        /// <param name="portfolioSymbolId">Id the portfolio symbol.</param>
        /// <param name="patches">Patch commands for update.</param>
        /// <response code="200">Portfolio stock symbol successfully updated.</response>
        /// <response code="400">Invalid patch data.</response>
        /// <response code="404">Portfolio or portfolio symbol not found.</response>
        [HttpPatch("api/portfolios/{portfolioId}/portfolioSymbols/{portfolioSymbolId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        /// <summary>
        /// Reorder portfolio symbols
        /// </summary>
        /// <param name="portfolioId">Id of portfolio</param>
        /// <param name="reorderPortfolioCommand">Data for the new order of the portfolio symbols.</param>
        /// <response code="200">Portfolio symbols successfully reordered.</response>
        /// <response code="404">Portfolio or portfolio symbols not found.</response>
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

        /// <summary>
        /// Delete the given portfolio symbol for a portfolio
        /// </summary>
        /// <param name="portfolioId">Id of the portfolio.</param>
        /// <param name="portfolioSymbolid">Id of the portfolio symbol to delete.</param>
        /// <response code="200">Portfolio symbol successfully deleted.</response>
        [HttpDelete("api/portfolios/{portfolioId}/portfolioSymbols/{portfolioSymbolid}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> DeletePortfolioSymbol(int portfolioId, int portfolioSymbolid)
        {
            await _portfolioRepository.DeletePortfolioSymbol(portfolioSymbolid);

            return Ok();
        }
    }
}
