using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class DownloadedSymbol
    {
        public string Ticker { get; set; }

        public int ExchangeId { get; set; }

        public string FullName { get; set; }
    }
}
