using FomoAPI.Application.Exchanges;
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

        private FomoApiApplicationFactory _fomoApiFactory;

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
            if(_fomoApiFactory == null)
            {
                _fomoApiFactory = new FomoApiApplicationFactory(configureServices);
                return true;
            }

            return false;
        }

        public HttpClient GetClientNotAuth()
        {
            if(_httpClientNoAuth == null)
            {
                _httpClientNoAuth = _fomoApiFactory.CreateClient(new WebApplicationFactoryClientOptions
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

            if (_fomoApiFactory != null)
            {
                _fomoApiFactory.Dispose();
                _fomoApiFactory = null;
            }

            Disposed = true;
        }
    }
}
