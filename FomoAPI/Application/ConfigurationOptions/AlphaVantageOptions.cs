using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class AlphaVantageOptions
    {
        [Required]
        public string ApiKey { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Url]
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
