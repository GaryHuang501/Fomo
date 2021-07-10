using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class PortfolioStocksUpdateOptions
    {
        [Range(60, int.MaxValue)]
        public int IntervalSeconds { get; set; }

        [Range(1, 100000)]
        public int BatchSize { get; set; }
    }
}
