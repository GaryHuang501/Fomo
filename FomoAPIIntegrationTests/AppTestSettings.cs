﻿using FomoAPI;
using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FomoAPIIntegrationTests
{
    /// <summary>
    /// Wrapper for settings for tests
    /// </summary>
    public class AppTestSettings
    {
        private static readonly Lazy<AppTestSettings> _instance = new Lazy<AppTestSettings>(new AppTestSettings());

        public static AppTestSettings Instance { get { return _instance.Value; } }

        private readonly IConfiguration _launchSettingsConfig;
        private readonly IConfiguration _firebaseSettingsConfig;
        private readonly IConfiguration _testAppSettingsConfig;

        public Guid TestUserId
        {
            get { return Guid.Parse(_testAppSettingsConfig["TestUser:UserId"]); }
        }

        public string TestDBConnectionString
        {
            get { return _testAppSettingsConfig["Database:ConnectionString"]; }
        }

        public AlphaVantageOptions AlphaVantageOptions
        {
            get { return _testAppSettingsConfig.GetSection("AlphaVantage").Get<AlphaVantageOptions>(); }
        }

        public AlphaVantageOptions AlphaVantageLiveOptions
        {
            get { return _testAppSettingsConfig.GetSection("AlphaVantageLive").Get<AlphaVantageOptions>(); }
        }

        public EventBusOptions EventBusOptions
        {
            get { return _testAppSettingsConfig.GetSection("EventBus").Get<EventBusOptions>(); }
        }

        public FireBaseOptions FireBaseOptions
        {
            get { return _firebaseSettingsConfig.GetSection("FireBase").Get<FireBaseOptions>(); }
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

        public int DefaultBulkCopyBatchSize
        {
            get { return int.Parse(_testAppSettingsConfig["Database:DefaultBulkCopyBatchSize"]); }
        }
        public AppTestSettings()
        {
            _testAppSettingsConfig = CreateConfiguration("appsettings.Test.json");
            _launchSettingsConfig = CreateConfiguration("launchSettings.json");
            _firebaseSettingsConfig = CreateConfiguration("firebase.json");
        }

        private IConfiguration CreateConfiguration(string fileName)
        {
            return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(fileName, optional: true, reloadOnChange: true)
            .AddUserSecrets<AppTestSettings>()
            .Build();
        }
    }
}
