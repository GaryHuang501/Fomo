using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// A Stock Symbol such as TSLA, MSFT, AAPL.
    /// </summary>
    public class Symbol
    {
        public int Id { get; set; }

        public string Ticker { get; set; }

        public string ExchangeName { get; set; }

        public int ExchangeId { get; set; }

        public string FullName { get; set; }

        public bool Delisted { get; set; }
    }
}
