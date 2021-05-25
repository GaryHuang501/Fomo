using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FomoAPI.Application.Commands.Vote;
using FomoAPI.Controllers.Authorization;
using FomoAPI.Domain.Stocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VotesController : ControllerBase
    {

        private ILogger<VotesController> _logger;
        private IVoteRepository _voteRepo;

        public VotesController(
                                 IVoteRepository voteRepo, 
                                 ILogger<VotesController> logger)
        {
            _voteRepo = voteRepo;
            _logger = logger;
        }

        [HttpPost()]
        public async Task<IActionResult> SaveVote(VoteCommand command)
        {
            Guid userId = User.GetUserId();
            var vote = new Vote(userId, command.SymbolId, command.Direction, DateTime.UtcNow);

            await _voteRepo.SaveVote(vote);

            return Ok();
        }

        [HttpGet()]
        public async Task<IReadOnlyDictionary<int, TotalVotes>> GetTotalVotes([FromQuery] int[] sids)
        {
            Guid userId = User.GetUserId();

            var symbolIds = new HashSet<int>(sids);
            return await _voteRepo.GetTotalVotes(symbolIds, userId);
        }

    }
}