using FomoAPI.Infrastructure.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class NasdaqParser : IExchangeParser
    {
        private readonly ILogger<NasdaqParser> _logger;

        public NasdaqParser(ILogger<NasdaqParser> logger)
        {
            _logger = logger;
        }

        public async Task<IDictionary<string, DownloadedSymbol>> GetTickerToSymbolMap(StreamReader reader, string delimiter, string[] suffixBlackList)
        {
            var tickerToSymbolMap = new Dictionary<string, DownloadedSymbol>();
            var headers = reader.ReadLine().Split(delimiter);

            int symbolIndex = GetHeaderIndex("Symbol", headers);
            int securityNameIndex = GetHeaderIndex("Security Name", headers);
            int exchangeIndex = GetHeaderIndex("Listing Exchange", headers);

            string row;

            while ((row = await reader.ReadLineAsync()) != null)
            {
                var columns = row.Split(delimiter);
                var fullName = FilterFullName(columns[securityNameIndex].Trim(), suffixBlackList);
                var exchangeType = ToExchangeType(columns[exchangeIndex].Trim());
                var ticker = columns[symbolIndex].Trim();


                if (exchangeType == ExchangeType.Unknown)
                {
                    _logger.LogInformation("Exchange {exchangeName} not supported. Ignoring ticker {ticker}.", columns[exchangeIndex], columns[symbolIndex]);
                    continue;
                }

                var symbol = new DownloadedSymbol
                {
                    Ticker = ticker,
                    FullName = fullName,
                    ExchangeId = exchangeType.Id,
                };

                _logger.LogTrace("Added ticker {ticker} for exchange {exchange}", columns[symbolIndex], columns[exchangeIndex]);

                tickerToSymbolMap.Add(symbol.Ticker, symbol);
            }

            if (tickerToSymbolMap.Keys.Count == 0) 
                throw new ArgumentException("No parseable data received from Nasdaq");

            return tickerToSymbolMap;
        }

        /// <summary>
        /// Filter the full name to only include part of name before the given suffixes.
        /// </summary>
        /// <param name="fullName">the name to filter</param>
        /// <param name="suffixFilters">suffixes to filter by</param>
        /// <returns>filtered string</returns>
        private string FilterFullName(string fullName, string[] suffixFilters)
        {
            foreach(var suffix in suffixFilters)
            {
                var index = fullName.IndexOf(suffix);

                if(index != -1)
                {
                    return fullName.Substring(0, index).Trim();
                }
            }

            return fullName;
        }


        private int GetHeaderIndex(string expectedColumn, string[] headers)
        {
            int index = Array.FindIndex<string>(headers, (c) => c == expectedColumn);

            if (index < 0 )
            {
                throw new FormatException($"{expectedColumn} header could not be found in nasdaq traded file");
            }

            return index;
        }

        /// <summary>
        /// Converts the exchange symbol to the actual name that matches the Fomo system.
        /// </summary>
        /// <param name="exchanageNameSymbol">Exchange Symbol from Nasdaq</param>
        /// <returns>Fomo Exchange Name. Null if it's an unsupported exchange.</returns>
        private ExchangeType ToExchangeType(string exchanageNameSymbol)
        {
            switch (exchanageNameSymbol)
            {
                case "Q": return ExchangeType.NASDAQ;
                case "N": return ExchangeType.NYSE;
                case "P": return ExchangeType.NYSEARCA;
                case "A": return ExchangeType.NYSEAMERICAN;
                default: return ExchangeType.Unknown;
            }
        }
    }
}
