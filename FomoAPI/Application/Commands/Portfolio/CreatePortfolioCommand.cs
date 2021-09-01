using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    /// <summary>
    /// Command to create a new portfolio for a user
    /// </summary>
    public class CreatePortfolioCommand
    {
        public string Name { get; set; }
    }
}
