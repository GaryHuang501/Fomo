using FomoAPI.Application.Exchanges;
using FomoAPI.Application.Stores;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace FomoAPIIntegrationTests.Fixtures
{
    public class FomoApiFixture : IDisposable
    {
        private HttpClient _httpClientNoAuth;

        public FomoApiApplicationFactory FomoApiFactory { get; private set; }

        public bool Disposed { get; private set; }

        public FomoApiFixture()
        {

        }

        public static void WithNoHostedServices(IServiceCollection services)
        {
            services.RemoveAll(typeof(IHostedService));
        }

        public static void WithNoExchangeSync(IServiceCollection services)
        {
            services.RemoveAll(typeof(ExchangeSyncHostedService));
        }

        public bool CreateServer(Action<IServiceCollection> configureServices)
        {
            if(FomoApiFactory == null)
            {
                FomoApiFactory = new FomoApiApplicationFactory(configureServices);
                return true;
            }

            return false;
        }

        public HttpClient GetClientNotAuth()
        {
            if(_httpClientNoAuth == null)
            {
                _httpClientNoAuth = FomoApiFactory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
            }

            return _httpClientNoAuth;
        }

        public void Dispose()
        {
            if (_httpClientNoAuth != null)
            {
                _httpClientNoAuth.Dispose();
                _httpClientNoAuth = null;
            }

            if (FomoApiFactory != null)
            {
                FomoApiFactory.Dispose();
                FomoApiFactory = null;
            }

            Disposed = true;
        }
    }
}
