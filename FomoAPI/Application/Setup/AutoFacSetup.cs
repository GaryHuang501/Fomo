using Autofac;
using Autofac.Extensions.DependencyInjection;
using FomoAPI.Application.Stores;
using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryExecutorContexts;
using FomoAPI.Infrastructure.AlphaVantage;
using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FomoAPI.Application
{
    public class AutoFacSetup
    {
        public static IContainer Setup(IServiceCollection services, IConfiguration configuration)
        {
            var builder = new ContainerBuilder();

            RegisterOptions(services, configuration);
            builder.Populate(services);

            RegisterServices(builder);
            RegisterComponents(builder);

            return builder.Build();
        }

        private static void RegisterOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AlphaVantageOptions>(configuration.GetSection("AlphaVantage"));
            services.Configure<EventBusOptions>(configuration.GetSection("EventBus"));
            services.Configure<SingleQuoteCacheOptions>(configuration.GetSection("SingleQuoteCache"));
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<QueryEventBusTimedHostedService>().As<IHostedService>().SingleInstance();
        }

        private static void RegisterComponents(ContainerBuilder builder)
        {
            builder.RegisterType<QueryEventBus>().SingleInstance();
            builder.RegisterType<QueryExecutorContextRegistry>().SingleInstance();
            builder.RegisterType<QuerySubscriptions>().SingleInstance();

            builder.RegisterType<AlphaVantageSingleQuoteQueryExecutorContext>();
            builder.RegisterType<AlphaVantageParserFactory>().As<IAlphaVantageDataParserFactory>();
            builder.RegisterType<AlphaVantageClient>().As<IAlphaVantageClient>();
        }
    }
}
