﻿using FomoAPI.Application.Exchanges;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace FomoAPIIntegrationTests.Fixtures
{
    public class FomoApiFixture : IDisposable
    {
        public HttpClient HttpClientNoAuth { get; private set; }

        private FomoApiApplicationFactory _fomoApiFactory;

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
            if(HttpClientNoAuth == null)
            {
                HttpClientNoAuth = _fomoApiFactory.CreateClient(new WebApplicationFactoryClientOptions
                {
                    AllowAutoRedirect = false,
                });
            }

            return HttpClientNoAuth;
        }

        public void Dispose()
        {
            if(HttpClientNoAuth != null)
            {
                HttpClientNoAuth.Dispose();
                HttpClientNoAuth = null;
            }

            if (_fomoApiFactory != null)
            {
                _fomoApiFactory.Dispose();
                _fomoApiFactory = null;
            }
        }
    }
}
