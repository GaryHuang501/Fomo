using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Exchange client to retrieve list of all traded symbols
    /// </summary>
    public class ExchangeClient : IExchangeClient
    {
        private readonly IExchangeParser _parser;

        private readonly ILogger _logger;

        private readonly IFtpClient _ftpClient;

        public ExchangeClient(IFtpClient ftpClient, IExchangeParser parser, ILogger<ExchangeClient> logger)
        { 
            _ftpClient = ftpClient;
            _parser = parser;
            _logger = logger;
        }

        /// <summary>
        /// Get all the listed traded symbols for for all major stock exchanges from nasdaq ftp
        /// </summary>
        /// <param name="syncSettings">The <see cref="ExchangeSyncSetting"/> containing sync settings.</ExchangeSyncSetting> </param>
        /// <returns>IReadOnlyDictionary with <see cref="SymbolKey"/> as key and <see cref="DownloadedSymbol"/> for the value.</returns>
        public async Task<IReadOnlyDictionary<SymbolKey, DownloadedSymbol>> GetTradedSymbols(ExchangeSyncSetting syncSettings)
        {
            _logger.LogTrace("Executing query http request url: {url}", syncSettings.Url);

            // no login necessary
            using var stream = _ftpClient.DownloadFile(syncSettings.Url, string.Empty, string.Empty);
            using StreamReader reader = new StreamReader(stream);

            var tickerToSymbols = await _parser.GetSymbolMap(reader, syncSettings.Delimiter, syncSettings.SuffixBlackList);
  
            return new ReadOnlyDictionary<SymbolKey, DownloadedSymbol>(tickerToSymbols);
        }
    }
}
