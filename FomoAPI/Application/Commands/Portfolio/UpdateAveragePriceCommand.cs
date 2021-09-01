using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Commands.Portfolio
{
    /// <summary>
    /// Command to set the average price of a stock symbol in a portfolio
    /// </summary>
    public class UpdateAveragePriceCommand
    {
        public decimal AveragePrice { get; set; }
    }
}
