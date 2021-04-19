using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMemberQueries _memberQueries;

        public MembersController(IMemberQueries memberQueries)
        {
            _memberQueries = memberQueries;
        }

        public async Task<ActionResult<MembersViewModel>> GetMembers([FromQuery] int limit, [FromQuery] int offset)
        {
            var viewModel = await _memberQueries.GetPaginatedMembers(limit, offset);

            return Ok(viewModel);
        }

    }
}
