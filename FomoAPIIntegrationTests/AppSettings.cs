using System;
using System.Collections.Generic;
using System.IO;
using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Configuration;

namespace FomoAPIIntegrationTests
{
    public class AppSettings
    {
        public static readonly Lazy<AppSettings> Instance = new Lazy<AppSettings>(new AppSettings());

        private readonly IConfiguration _config;

        public AlphaVantageOptions AlphaVantageOptions {
            get { return _config.GetSection("AlphaVantage").Get<AlphaVantageOptions>(); }
        }

 
        public AppSettings()
        {
             _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();
        }

        private void GetFomoApiBasePath()
        {

        }
    }
}
