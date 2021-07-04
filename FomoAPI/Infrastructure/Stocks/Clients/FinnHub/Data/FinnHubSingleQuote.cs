using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks.Clients.FinnHub.Data
{
    /// <summary>
    /// 5 minute interval candle stick to represent a single quote data.
    /// </summary>
    public class FinnHubSingleQuote
    {
        [JsonProperty("o")]
        public decimal OpenPrice { get; private set; }

        [JsonProperty("l")]
        public decimal LowPrice { get; private set; }

        [JsonProperty("h")]
        public decimal HighPrice { get; private set; }

        [JsonProperty("c")]
        public decimal CurrentPrice { get; private set; }

        [JsonProperty("pc")]
        public decimal PreviousClose { get; private set; }

        [JsonConstructor]
        public FinnHubSingleQuote(
            decimal openPrice, 
            decimal lowPrice, 
            decimal highPrice, 
            decimal currentPrice,
            decimal previousClose)
        {
            // API returns 0 if symbol is not found.
            if (currentPrice <= 0) throw new ArgumentOutOfRangeException("Current Price must be greater than 0");
            if (previousClose <= 0) throw new ArgumentOutOfRangeException("Previous Close must be greater than 0");

            OpenPrice = openPrice;
            LowPrice = lowPrice;
            HighPrice = highPrice;
            PreviousClose = previousClose;
            CurrentPrice = currentPrice;
        }

        public SingleQuoteData ToDomain(int symbolId, string ticker)
        {
            return new SingleQuoteData(
                    symbolId: symbolId,
                    ticker: ticker,
                    price: CurrentPrice,
                    change: CurrentPrice - PreviousClose,
                    changePercent: ((CurrentPrice - PreviousClose) / PreviousClose) * 100, // comes in decimal format
                    lastUpdated: DateTime.UtcNow
                );
 
        }
    }
}
