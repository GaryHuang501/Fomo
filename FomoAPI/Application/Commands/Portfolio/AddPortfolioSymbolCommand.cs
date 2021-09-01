using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    /// <summary>
    /// Command to add new stock symbol to a portfolio
    /// </summary>
    public class AddPortfolioSymbolCommand
    {
        public int SymbolId { get; set; }
    }
}
