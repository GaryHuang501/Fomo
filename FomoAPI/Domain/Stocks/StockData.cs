using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public abstract class StockData
    {
        public DateTime LastUpdated { get; protected set; }
    }
}
