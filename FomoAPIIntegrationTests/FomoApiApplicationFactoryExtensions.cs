using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FomoAPIIntegrationTests
{
    public static class FomoApiApplicationFactoryExtensions
    {
        public static HttpClient CreateClientNoAuth(this FomoApiApplicationFactory factory, string apiEndPoint = null)
        {
            var client =  factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
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
