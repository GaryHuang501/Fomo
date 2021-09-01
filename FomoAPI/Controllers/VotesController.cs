using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FomoAPI.Application.Commands.Vote;
using FomoAPI.Controllers.Authorization;
using FomoAPI.Domain.Stocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    /// <summary>
    /// Manages the votes for a user and stock symbol.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VotesController : ControllerBase
    {

        private readonly ILogger<VotesController> _logger;
        private readonly IVoteRepository _voteRepo;

        public VotesController(
                                 IVoteRepository voteRepo, 
                                 ILogger<VotesController> logger)
        {
            _voteRepo = voteRepo;
            _logger = logger;
        }

        /// <summary>
        /// Adds new vote for a symbol and user.
        /// </summary>
        /// <param name="command">Voting data.</param>
        /// <response code="200">Vote successfully casted.</response>
        /// <response code="404">User or symbol not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost()]
        public async Task<IActionResult> SaveVote(VoteCommand command)
        {
            Guid userId = User.GetUserId();
            var vote = new Vote(userId, command.SymbolId, command.Direction, DateTime.UtcNow);

            bool success = await _voteRepo.SaveVote(vote);

            if (!success)
            {
                return NotFound("User or symbol not found.");
            }

            return Ok();
        }

        /// <summary>
        /// Gets the voting data for the given symbols.
        /// </summary>
        /// <param name="sids">Collection of symbol ids to fetch the voting data for.</param>
        /// <response code="200">Returns the voting data for the symbol ids.</response>
        [Produces("application/json")]
        [HttpGet()]
        public async Task<IReadOnlyDictionary<int, TotalVotes>> GetTotalVotes([FromQuery] int[] sids)
        {
            Guid userId = User.GetUserId();

            var symbolIds = new HashSet<int>(sids);
            return await _voteRepo.GetTotalVotes(symbolIds, userId);
        }
    }
}