using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks.Queries
{
    public enum QueryStatus
    {
        Pending = 0,
        Executing = 1,
        Executed = 2
    }
}
