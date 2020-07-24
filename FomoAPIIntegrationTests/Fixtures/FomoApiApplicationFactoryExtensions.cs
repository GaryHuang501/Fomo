using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FomoAPIIntegrationTests.Fixtures
{
    public static class FomoApiApplicationFactoryExtensions
    {
        public static HttpClient CreateClientNoAuth(this FomoApiApplicationFactory factory, string apiEndPoint = null)
        {
            var client =  factory.WithWebHostBuilder(builder =>
            {
                builder
                       .ConfigureTestServices(services =>
                {
                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultSignInScheme = "Test";
                    })
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Test");
            return client;
        }
    }
}
