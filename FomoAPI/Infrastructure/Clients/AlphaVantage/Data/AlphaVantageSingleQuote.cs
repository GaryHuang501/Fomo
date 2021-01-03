using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Data
{
    public class AlphaVantageSingleQuote
    {
        [JsonProperty("Global Quote")]
        public GlobalQuote Data { get; set; }

        public SingleQuoteData ToDomain(int symbolId)
        {
            return new SingleQuoteData(
                    symbolId: symbolId,
                    ticker: Data.Symbol,
                    high: Data.High,
                    low: Data.Low,
                    open: Data.Open,
                    price: Data.Price,
                    previousClose: Data.PreviousClose,
                    volume: Data.Volume,
                    change: Data.Change,
                    changePercent: decimal.Parse(Data.ChangePercent.Trim().Replace("%", string.Empty)),
                    lastUpdated: Data.LastUpdated,
                    lastTradingDay: Data.LastTradingDay
                );
        }
    }
}
