using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <summary>
    /// Error JSON object returned by AlphaVantage when query fails
    /// </summary>
    public class AlphaVantageQueryError
    {
        [JsonProperty("Error Message", Required = Required.Always)]
        public string ErrorMessage { get; private set; }

        public AlphaVantageQueryError(string error)
        {
            ErrorMessage = error;
        }
    }
}
