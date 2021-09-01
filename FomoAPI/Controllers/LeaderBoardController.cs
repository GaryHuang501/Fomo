using System;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.ViewModels.LeaderBoard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FomoAPI.Controllers
{
    /// <summary>
    /// Displays leader board data for user rankings
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaderBoardController : ControllerBase
    {
        private readonly ILeaderBoardQueries _leaderBoardQueries;

        public LeaderBoardController(ILeaderBoardQueries leaderBoardQueries)
        {
            _leaderBoardQueries = leaderBoardQueries;
        }

        /// <summary>
        /// Get the leader board rankings for the leader board page.
        /// </summary>
        /// <response code="200">Returns the leader board rankings</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(LeaderBoardViewModel), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<ActionResult<LeaderBoardViewModel>> Get([FromQuery] int limit)
        {
            var viewModel = await _leaderBoardQueries.GetLeaderBoardData(limit);

            return Ok(viewModel);
        }
    }
}
