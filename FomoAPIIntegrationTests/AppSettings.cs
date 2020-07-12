using System;
using System.IO;
using FomoAPI.Application.ConfigurationOptions;
using Microsoft.Extensions.Configuration;

namespace FomoAPIIntegrationTests
{
    /// <summary>
    /// Settings wrapper for settings from actual App
    /// </summary>
    public class AppSettings
    {
        private static readonly Lazy<AppSettings> _instance = new Lazy<AppSettings>(new AppSettings());

        public static AppSettings Instance { get { return _instance.Value; } }

        private readonly IConfiguration _appSettingsConfig;

        private readonly IConfiguration _launchSettingsConfig;


        public AlphaVantageOptions AlphaVantageOptions
        {
            get { return _appSettingsConfig.GetSection("AlphaVantage").Get<AlphaVantageOptions>(); }
        }


        public string FomoApiURL
        {
            get
            {
                var uriBuilder = new UriBuilder(_launchSettingsConfig["iisSettings:iisExpress:applicationUrl"])
                {
                    Scheme = Uri.UriSchemeHttps,
                    Port = int.Parse(_launchSettingsConfig["iisSettings:iisExpress:sslPort"])
                };

                return uriBuilder.ToString();
            }
        }

        public AppSettings()
        {
            _appSettingsConfig = CreateConfiguration("appSettings.json");
            _launchSettingsConfig = CreateConfiguration("launchSettings.json");
        }

        private IConfiguration CreateConfiguration(string fileName)
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(fileName, optional: true, reloadOnChange: true)
            .Build();
        }
    }
}
