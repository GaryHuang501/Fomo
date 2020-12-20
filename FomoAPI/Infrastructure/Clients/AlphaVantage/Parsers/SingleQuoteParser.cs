using System;
using System.Collections.Generic;
using System.IO;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage.Data;
using Newtonsoft.Json;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers
{
    /// <summary>
    /// Parser class used in AlphaVantage to map CSV query response data to StockSingleQuoteData.
    /// </summary>
    public class SingleQuoteParser : IAlphaVantageDataParser<StockSingleQuoteData>
    {
        private readonly JsonSerializerSettings _settings;

        public SingleQuoteParser()
        {
            _settings = new JsonSerializerSettings();
            _settings.MissingMemberHandling = MissingMemberHandling.Error;
            _settings.NullValueHandling = NullValueHandling.Include;
        }

        public StockSingleQuoteData ParseJson(string json)
        {
            var data =  JsonConvert.DeserializeObject<AlphaVantageSingleQuote>(json, _settings);
            return data.ToDomain();
        }

        /// <summary>
        /// Parse the csv data to StockSingleQuotedataCsvParser.
        /// Will throw exception if any field is missing.
        /// </summary>
        /// <param name="reader">Reader holding the response data</param>
        /// <returns></returns>
        public StockSingleQuoteData ParseCsv(StreamReader reader)
        {
            string headerLine = reader.ReadLine();

            if (string.IsNullOrEmpty(headerLine)) throw new ArgumentException(nameof(headerLine));

            var headers = headerLine.Split(",");

            var indexToColumnHeaderMap = new Dictionary<int, string>();

           for (int i = 0; i < headers.Length; i++) 
            {
                var header = headers[i];
                indexToColumnHeaderMap.Add(i, header);
            }

            string valuesLine = reader.ReadLine();

            if (string.IsNullOrEmpty(valuesLine)) throw new ArgumentException(nameof(valuesLine));

            var values = valuesLine.Split(",");

            var columnHeaderValueMap = new Dictionary<string, string>();

            for (int i = 0; i < values.Length; i++)
            {
                var header = indexToColumnHeaderMap[i];
                var value = values[i];

                columnHeaderValueMap.Add(header, value);
            }

            var mappedData = MapData(columnHeaderValueMap);

            return mappedData;
        }

        private StockSingleQuoteData MapData(Dictionary<string, string> columnHeaderValueMap)
        {
            string symbol = columnHeaderValueMap[nameof(symbol)];
            decimal high = decimal.Parse(columnHeaderValueMap[nameof(high)]);
            decimal low = decimal.Parse(columnHeaderValueMap[nameof(low)]);
            decimal open = decimal.Parse(columnHeaderValueMap[nameof(open)]);
            decimal previousClose = decimal.Parse(columnHeaderValueMap[nameof(previousClose)]);
            long volume = long.Parse(columnHeaderValueMap[nameof(volume)]);
            decimal change = decimal.Parse(columnHeaderValueMap[nameof(change)]);
            string changePercent = columnHeaderValueMap[nameof(changePercent)];
            decimal price = decimal.Parse(columnHeaderValueMap[nameof(price)]);
            DateTime latestDay = DateTime.Parse(columnHeaderValueMap[nameof(latestDay)]);

            return new StockSingleQuoteData(
                    symbol: symbol,
                    high: high,
                    low: low,
                    open: open,
                    previousClose: previousClose,
                    volume: volume,
                    change: change,
                    price: price,
                    changePercent: decimal.Parse(changePercent.Trim().Replace("%", string.Empty)),
                    lastTradingDay: latestDay,
                    lastUpdated: DateTime.UtcNow
                );
        }
    }
}
