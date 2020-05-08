using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Exchange client to retrieve list of all traded symbols
    /// </summary>
    public class ExchangeClient : IExchangeClient
    {
        private readonly IExchangeOptions _exchangeOptions;

        private readonly IExchangeParser _parser;

        private readonly ILogger _logger;

        private readonly IFtpClient _ftpClient;

        public ExchangeClient(IFtpClient ftpClient, IOptionsMonitor<IExchangeOptions> optionsAccessor, IExchangeParser parser, ILogger<ExchangeClient> logger)
        { 
            _ftpClient = ftpClient;
            _exchangeOptions = optionsAccessor.CurrentValue;
            _parser = parser;
            _logger = logger;
        }

        /// <summary>
        /// Get all the listed traded symbols for for all major stock exchanges from nasdaq ftp
        /// </summary>
        /// <returns>Dictionary with Ticker Symbol as key and the downloaded symbol as the value.</returns>
        public async Task<IDictionary<string, DownloadedSymbol>> GetTradedSymbols()
        {
            IDictionary<string, DownloadedSymbol> tickerToSymbolMap = new Dictionary<string, DownloadedSymbol>();

            _logger.LogTrace("Executing query http request url: {url}", _exchangeOptions.Url);

            // no login necessary
            using var stream = _ftpClient.DownloadFile(_exchangeOptions.Url, string.Empty, string.Empty);
            using StreamReader reader = new StreamReader(stream);

            tickerToSymbolMap = await _parser.GetTickerToSymbolMap(reader, _exchangeOptions.Delimiter, _exchangeOptions.SuffixBlackList);
  
            return tickerToSymbolMap;
        }
    }
}
