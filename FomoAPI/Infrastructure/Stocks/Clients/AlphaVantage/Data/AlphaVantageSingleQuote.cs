using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Data
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
                    price: Data.Price,
                    change: Data.Change,
                    changePercent: decimal.Parse(Data.ChangePercent.Trim().Replace("%", string.Empty)),
                    lastUpdated: Data.LastUpdated
                );
        }
    }
}
