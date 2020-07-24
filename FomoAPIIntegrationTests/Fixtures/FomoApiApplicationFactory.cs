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

        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = base.CreateHostBuilder();


            builder.ConfigureWebHost(webBuilder =>
                    {
                        webBuilder
                            .UseEnvironment("Test")
                            .ConfigureTestServices(services =>
                            {
                                services.RemoveAll(typeof(IHostedService));
                            });
                    });

            return builder;
        }
    }
}
