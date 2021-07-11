using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// The buffer room time that stock data may have its data updates delayed.
    /// </summary>
    /// <remarks>
    /// For example, stock data is not updated instantly on the third party provider. The market closing value
    /// may not appear until minutes after closing.
    /// </remarks>
    public class StockUpdateTimeBufferRoom
    {
        public const int Minutes = 10;

        public const int Seconds = 600;
    }
}
