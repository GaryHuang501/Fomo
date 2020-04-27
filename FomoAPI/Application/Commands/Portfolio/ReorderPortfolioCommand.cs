using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    public class ReorderPortfolioCommand
    {
        public Dictionary<string, int> PortfolioSymbolIdToSortOrder { get; set; }
    }
}
