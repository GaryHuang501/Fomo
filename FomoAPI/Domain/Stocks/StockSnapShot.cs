using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Snapshot data for a specific instance in time for a stock.
    /// </summary>
    /// <remarks>Used when looking at stocks over a time interval.</remarks>
    public class StockSnapShot
    {
        public StockSnapShot(decimal high, decimal low, decimal close, decimal open, long volume)
        {
            High = high;
            Low = low;
            Close = close;
            Open = open;
            Volume = volume;
        }

        public decimal High { get; private set; }

        public decimal Low { get; private set; }

        public decimal Close { get; private set; }

        public decimal Open { get; private set; }

        public long Volume { get; private set; }
    }
}
