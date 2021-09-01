using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.ViewModels;
using FomoAPI.Application.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FomoAPI.Controllers
{
    /// <summary>
    /// Displays the view model for the members page.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MembersController : ControllerBase
    {
        private readonly IMemberQueries _memberQueries;

        public MembersController(IMemberQueries memberQueries)
        {
            _memberQueries = memberQueries;
        }

        /// <summary>
        /// Gets the users groupings for the member page.
        /// </summary>
        /// <response code="200">Returns the user groupings.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(MembersViewModel), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<ActionResult<MembersViewModel>> GetMembers([FromQuery] int limit, [FromQuery] int offset)
        {
            var viewModel = await _memberQueries.GetPaginatedMembers(limit, offset);

            return Ok(viewModel);
        }

    }
}
