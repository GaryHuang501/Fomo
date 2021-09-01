using System;
using System.Collections.Generic;
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
    /// Manages the user's portfolio of stock symbols.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfolioRepository _portfolioRepository;
        private readonly PortfolioValidator _validator;
        private readonly ILogger<PortfoliosController> _logger;

        public PortfoliosController(IPortfolioRepository portfolioRepository, PortfolioValidator validator, ILogger<PortfoliosController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Get the portfolio by Id.
        /// </summary>
        /// <response code="200">Returns the specified portfolio.</response>
        /// <response code="404">No matching portoflio found.</response>
        /// <param name="id">Unique id for the portfolio.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return Ok(portfolio);
        }

        /// <summary>
        /// Get the collection of portfolio ids that the given user owns.
        /// </summary>
        /// <response code="200">Returns portfolio ids</response>
        /// <response code="404">No matching user found or user has no portfolios created.</response>
        /// <param name="userId">Id of the portfolio owner.</param>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<int>>> GetPortfolioIds([FromQuery]string userId)
        {
            if(!Guid.TryParse(userId, out Guid userGuid))
            {
                return BadRequest("UserId must be a valid GUID");
            }

            var ids = await _portfolioRepository.GetPortfolioIds(userGuid);

            if (ids == null)
            {
                return NotFound();
            }

            return Ok(ids);
        }

        /// <summary>
        /// Creates a new portfolio.
        /// </summary>
        /// <response code="200">Portfolio Created.</response>
        /// <param name="createPortfolioCommand">New portfolio data.</param>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<Portfolio>> CreateAsync([FromBody] CreatePortfolioCommand createPortfolioCommand)
        {
            var userId = User.GetUserId();
            var newPortfolio =  await _portfolioRepository.CreatePortfolio(userId, createPortfolioCommand.Name);

            return Ok(newPortfolio);
        }


        /// <summary>
        /// Deletes the specified portfolio.
        /// </summary>
        /// <response code="200">Portfolio deleted.</response>
        /// <param name="id">Id of portfolio to delete.</param>
        [HttpDelete("{id}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _portfolioRepository.DeletePortfolio(id);

            return Ok();
        }

        /// <summary>
        /// Patch update the specified portfolio.
        /// </summary>
        /// <response code="200">Portfolio successfully patched.</response>
        /// <response code="404">Portfolio not found.</response>
        /// <param name="id">Id of portfolio to patch.</param>
        /// <param name="patches">Update patch commands.</param>
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> UpdatePortfolio(int id, [FromBody] JsonPatchDocument<Portfolio> patches)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(id);

            if (portfolio == null)
            {
                return NotFound("Portfolio does not exist");
            }

            var updatedPortfolio = patches.CopyTo(portfolio);

            var validationResult = await _validator.ValidateAsync(portfolio);

            if (!validationResult.IsValid)
            {
                return BadRequest($"Patches updates values are invalid: {validationResult}");
            }

            var success = await _portfolioRepository.UpdatePortfolio(updatedPortfolio);

            if (!success)
            {
                return NotFound("Portfolio was not successfully updated");
            }

            return Ok();
        }
    }
}
