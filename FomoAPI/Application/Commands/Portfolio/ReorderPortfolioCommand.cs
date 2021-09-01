using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    /// <summary>
    /// Command to reorder the stocks on a portfolio to a given position
    /// </summary>
    public class ReorderPortfolioCommand
    {
        public Dictionary<int, int> PortfolioSymbolIdToSortOrder { get; set; }
    }
}
