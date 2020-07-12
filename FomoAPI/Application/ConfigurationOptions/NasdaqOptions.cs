using System;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Application.ConfigurationOptions
{
    public class NasdaqOptions : IExchangeOptions
    {
        [Required]
        public string Url { get; set; }

        [Required]

        public string ClientName { get; set; }

        [Required]

        public string Delimiter { get; set; }

        public string[] SuffixBlackList { get; set; }
    }
}
