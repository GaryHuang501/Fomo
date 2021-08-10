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

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Portfolio>> GetPortfolio(int id)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return Ok(portfolio);
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        [HttpPost]
        public async Task<ActionResult<Portfolio>> CreateAsync([FromBody] CreatePortfolioCommand createPortfolioCommand)
        {
            var userId = User.GetUserId();
            var newPortfolio =  await _portfolioRepository.CreatePortfolio(userId, createPortfolioCommand.Name);

            return Ok(newPortfolio);
        }

        [HttpDelete("{id}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _portfolioRepository.DeletePortfolio(id);

            return Ok();
        }

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
