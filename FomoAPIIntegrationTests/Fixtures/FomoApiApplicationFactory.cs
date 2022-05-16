using FomoAPI.Application.Exchanges;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace FomoAPIIntegrationTests.Fixtures
{
    /// <summary>
    /// Factory to generate in-memory APi service and bypass authentication.
    /// </summary>
    public class FomoApiApplicationFactory: WebApplicationFactory<FomoAPI.Startup>
    {
        public ITestOutputHelper Output { get; set; }

        private Action<IServiceCollection> _configureServices;

        public FomoApiApplicationFactory(Action<IServiceCollection> configureServices)
        {
            _configureServices = configureServices;
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {

            builder.ConfigureWebHost(webBuilder =>
                    {
                        webBuilder
                            .UseEnvironment("Test")
                            .ConfigureTestServices(services =>
                            {
                                _configureServices?.Invoke(services);

                                // Disable authentication
                                services.AddAuthentication(options =>
                                {
                                    options.DefaultAuthenticateScheme = "Test";
                                    options.DefaultChallengeScheme = "Test";
                                    options.DefaultSignInScheme = "Test";
                                })
                                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                            });

                    });

            var host = base.CreateHost(builder);

            return host;
        }
    }
}
