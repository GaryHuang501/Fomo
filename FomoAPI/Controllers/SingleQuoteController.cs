using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FomoAPI.Application.Commands;
using FomoAPI.Infrastructure.AlphaVantage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FomoAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SingleQuoteController : ControllerBase
    {
        [HttpPost("Query")]
        public IActionResult CreateQuery([FromBody] CreateSingleQuoteQueryCommand command)
        {
            return Ok();
        }
    }
}