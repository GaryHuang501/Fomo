using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FomoAPI.Application.Commands.Portfolio;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PortfoliosController : ControllerBase
    {
        private IPortfolioRepository _portfolioRepository;

        private ILogger<PortfoliosController> _logger;

        public PortfoliosController(IPortfolioRepository portfolioRepository, ILogger<PortfoliosController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPortfolio(int id)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(id);

            if (portfolio == null)
            {
                return NotFound();
            }

            return Ok(portfolio);
        }

        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<int>>> GetPortfolioIds()
        {
            Guid userId = User.GetUserId();

            var ids = await _portfolioRepository.GetPortfolioIds(userId);

            if (ids == null)
            {
                return NotFound();
            }

            return Ok(ids);
        }

        [HttpPatch("{id}/rename")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RenameAsync(int id, [FromBody] RenamePortfolioCommand renamePortfolioCommand)
        {
            var success = await _portfolioRepository.RenamePortfolio(id, renamePortfolioCommand.Name);

            if (!success)
            {
                return NotFound("Portfolio does not exist");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<PortfolioRepository>> CreateAsync([FromBody] CreatePortfolioCommand createPortfolioCommand)
        {
            var userId = User.GetUserId();
            var newPortfolio =  await _portfolioRepository.CreatePortfolio(userId, createPortfolioCommand.Name);

            return Ok(newPortfolio);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            await _portfolioRepository.DeletePortfolio(id);

            return Ok();
        }

        [HttpPost("{id}/portfolioSymbols")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddPortfolioSymbol(int id, [FromBody] AddPortfolioSymbolCommand addPortfolioSymbolCommand)
        {          
           var portfolioSymbol = await _portfolioRepository.AddPortfolioSymbol(id, addPortfolioSymbolCommand.SymbolId);

            if (portfolioSymbol == null)
            {
                return BadRequest("Symbol does not exist or already exists in portfolio");
            }

            return Ok(portfolioSymbol);
        }

        [HttpPatch("{id}/portfolioSymbols/reorder")]
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
        public async Task<IActionResult> DeletePortfolioSymbol(int portfolioSymbolid)
        {
            await _portfolioRepository.DeletePortfolioSymbol(portfolioSymbolid);

            return Ok();
        }
    }
}
