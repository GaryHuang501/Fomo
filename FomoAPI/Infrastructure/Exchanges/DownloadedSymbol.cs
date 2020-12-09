using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Represents the downloaded symbol from the exchange client.
    /// </summary>
    public class DownloadedSymbol
    {
        public string Ticker { get; private set; }

        public int ExchangeId { get; private set; }

        private string _fullName;

        /// <summary>
        /// Gets the full name of the symbol
        /// </summary>
        /// <remarks>Truncates the full name for database to 400 characters.</remarks>
        public string FullName => _fullName.Length <= 400 ? _fullName : _fullName.Substring(0, 400);

        public DownloadedSymbol(string ticker, int exchangeId, string fullName)
        {
            if (ticker.Length > 20)
            {
                throw new ArgumentException(nameof(ticker), "Database only allows 20 characters for ticker");
            }

            Ticker = ticker;
            ExchangeId = exchangeId;
            _fullName = fullName;
        }
    }
}
