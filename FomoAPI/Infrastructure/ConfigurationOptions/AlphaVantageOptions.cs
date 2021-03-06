﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FomoAPI.Infrastructure.ConfigurationOptions
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
    }
}
