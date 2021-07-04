﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage
{
    /// <summary>
    /// Error JSON object returned by AlphaVantage when query fails
    /// </summary>
    public class AlphaVantageQueryError
    {
        [JsonProperty("Error Message", Required = Required.Always)]
        public string ErrorMessage { get; private set; }

        [JsonProperty(Required = Required.AllowNull)]
        public string Note { get; private set; }

        public AlphaVantageQueryError(string error, string note)
        {
            ErrorMessage = error;
            Note = note;
        }
    }
}
