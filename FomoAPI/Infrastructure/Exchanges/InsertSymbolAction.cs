using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Insert Action object for saving symbol fields to Database.
    /// </summary>
    public class InsertSymbolAction
    {
        public string Ticker { get; private set; }

        public int ExchangeId { get; private set; }

        public string FullName { get; private set; }

        public bool Delisted { get; private set; }

        public InsertSymbolAction(string ticker, int exchangeId, string fullName, bool delisted)
        {
            Ticker = ticker;
            ExchangeId = exchangeId;
            FullName = fullName;
            Delisted = delisted;
        }
    }
}
