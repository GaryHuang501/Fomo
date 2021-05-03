using System;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.ViewModels.LeaderBoard;
using Microsoft.AspNetCore.Mvc;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderBoardController : ControllerBase
    {
        private readonly ILeaderBoardQueries _leaderBoardQueries;

        public LeaderBoardController(ILeaderBoardQueries leaderBoardQueries)
        {
            _leaderBoardQueries = leaderBoardQueries;
        }

        public async Task<ActionResult<LeaderBoardViewModel>> GetMembers([FromQuery] int limit)
        {
            var viewModel = await _leaderBoardQueries.GetLeaderBoardData(limit);

            return Ok(viewModel);
        }
    }
}
