using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FomoAPIIntegrationTests
{
    /// <summary>
    /// Wrapper for settings for tests
    /// </summary>
    public class TestAppSettings
    {
        private static readonly Lazy<TestAppSettings> _instance = new Lazy<TestAppSettings>(new TestAppSettings());

        public static TestAppSettings Instance { get { return _instance.Value; } }

        private readonly IConfiguration _testAppSettingsConfig;

        private readonly IConfiguration _credentialsConfig;

        public Guid TestUserId
        {
            get { return Guid.Parse(_credentialsConfig["TestUser:UserId"]); }
        }

        public string TestDBConnectionString
        {
            get { return _testAppSettingsConfig["Database:ConnectionString"]; }
        }

        public int DefaultBulkCopyBatchSize
        {
            get { return int.Parse(_testAppSettingsConfig["Database:DefaultBulkCopyBatchSize"]); }
        }
        public TestAppSettings()
        {
            _testAppSettingsConfig = CreateConfiguration("testappsettings.json");
            _credentialsConfig = CreateConfiguration("credentials.json");
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
