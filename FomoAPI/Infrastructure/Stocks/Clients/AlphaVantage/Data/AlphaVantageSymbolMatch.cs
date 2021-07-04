using FomoAPI.Domain.Stocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Data
{
    public class AlphaVantageSymbolMatch
    {
        [JsonProperty("1. symbol")]
        public string Symbol { get; set; }

        [JsonProperty("2. name")]
        public string Name { get; set; }

        [JsonProperty("9. matchScore")]
        public decimal MatchScore { get; set; }
    }
}
