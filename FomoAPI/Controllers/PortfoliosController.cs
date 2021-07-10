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

        private readonly ILogger<PortfoliosController> _logger;

        public PortfoliosController(IPortfolioRepository portfolioRepository, ILogger<PortfoliosController> logger)
        {
            _portfolioRepository = portfolioRepository;
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

        [HttpPost("{id}/portfolioSymbols")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<ActionResult<PortfolioSymbol>> AddPortfolioSymbol(int id, [FromBody] AddPortfolioSymbolCommand addPortfolioSymbolCommand)
        {          
           var portfolioSymbol = await _portfolioRepository.AddPortfolioSymbol(id, addPortfolioSymbolCommand.SymbolId);

            if (portfolioSymbol == null)
            {
                return BadRequest("Symbol does not exist or already exists in portfolio");
            }

            return Ok(portfolioSymbol);
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

            if (!updatedPortfolio.IsValid())
            {
                return BadRequest("Patch updates values are invalid.");
            }

            var success = await _portfolioRepository.UpdatePortfolio(updatedPortfolio);

            if (!success)
            {
                return NotFound("Portfolio was not successfully updated");
            }

            return Ok();
        }

        [HttpPatch("{id}/portfolioSymbols/{portfolioSymbolid}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> UpdatePortfolioSymbol(int id, int portfolioSymbolid, [FromBody] JsonPatchDocument<PortfolioSymbol> patches)
        {
            var portfolioSymbol = await _portfolioRepository.GetPortfolioSymbol(portfolioSymbolid);

            if (portfolioSymbol == null)
            {
                return NotFound("Portfolio does not exist");
            }

            var updatedPortfolioSymbol = patches.CopyTo(portfolioSymbol);

            if (!updatedPortfolioSymbol.IsValid())
            {
                return BadRequest("Patches updates values are invalid.");
            }

            var success = await _portfolioRepository.UpdatePortfolioSymbol(updatedPortfolioSymbol);

            if (!success)
            {
                return NotFound("PortfolioSymbol was not updated");
            }

            return Ok();
        }

        [HttpPatch("{id}/portfolioSymbols/sortOrder")]
        [Authorize(PolicyTypes.PortfolioOwner)]  
        public async Task<IActionResult> ReorderPortfolioSymbol(int id, [FromBody] ReorderPortfolioCommand reorderPortfolioCommand)
        {
            var success = await _portfolioRepository.ReorderPortfolioSymbol(id, reorderPortfolioCommand.PortfolioSymbolIdToSortOrder);

            if (!success)
            {
                return NotFound("Portfolio or PortfolioSymbol was not found.");
            }
            return Ok();
        }

        [HttpDelete("{id}/portfolioSymbols/{portfolioSymbolid}")]
        [Authorize(PolicyTypes.PortfolioOwner)]
        public async Task<IActionResult> DeletePortfolioSymbol(int portfolioSymbolid)
        {
            await _portfolioRepository.DeletePortfolioSymbol(portfolioSymbolid);

            return Ok();
        }
    }
}
