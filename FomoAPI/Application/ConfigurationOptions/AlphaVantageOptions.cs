using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class AlphaVantageOptions
    {
        public string ApiKey { get; set; }

        public string ClientName { get; set; }

        public string Url { get; set; }

        public AlphaVantageOptions()
        {
        }

        public AlphaVantageOptions(AlphaVantageOptions original)
        {
            ApiKey = original.ApiKey;
            ClientName = original.ClientName;
            Url = original.Url;
        }
    }
}
