using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    /// <summary>
    /// Command to rename an existing portfolio
    /// </summary>
    public class RenamePortfolioCommand
    {
        public string Name { get; set; }
    }
}
