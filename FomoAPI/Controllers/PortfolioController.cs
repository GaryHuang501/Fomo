using System.Threading.Tasks;
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
    public class PortfolioController : ControllerBase
    {
        private IPortfolioRepository _portfolioRepository;

        private ILogger<PortfolioController> _logger;

        public PortfolioController(IPortfolioRepository portfolioRepository, ILogger<PortfolioController> logger)
        {
            _portfolioRepository = portfolioRepository;
            _logger = logger;
        }

        [HttpGet("{id:int}", Name = "Get")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync(int portfolioId)
        {
            var portfolio = await _portfolioRepository.GetPortfolio(portfolioId);

            if (portfolio == null)
            {
                return NotFound();
            }

            return Ok(portfolio);
        }

        [HttpPatch("{id:int}/rename")]
        public async Task<IActionResult> RenameAsync(int portfolioId, [FromBody] string newName)
        {
            var success = await _portfolioRepository.RenamePortfolio(portfolioId, newName);

            if (!success)
            {
                return NotFound("Portfolio does not exist");
            }

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] string newPortfolioName)
        {
            var userId = User.GetUserId();
            var newPortfolio =  await _portfolioRepository.CreatePortfolio(userId, newPortfolioName);

            return Ok(newPortfolio);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> AddPortfolioSymbol(int portfolioId, [FromBody] int symbolId)
        {          
           var success = await _portfolioRepository.AddPortfolioSymbol(portfolioId, symbolId);

            if (!success)
            {
                return NotFound("Portfolio or symbol does not exist.");
            }

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync(int portfolioId)
        {
            await _portfolioRepository.DeletePortfolio(portfolioId);

            return Ok();
        }
    }
}
