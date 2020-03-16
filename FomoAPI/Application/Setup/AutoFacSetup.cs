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
using FomoAPI.Application.EventBuses.QueuePriorityRules;

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
            builder.RegisterType<AlphaVantageSingleQuoteQueryExecutorContext>();
            builder.RegisterType<AlphaVantageParserFactory>().As<IAlphaVantageDataParserFactory>();
            builder.RegisterType<AlphaVantageClient>().As<IAlphaVantageClient>();

            builder.RegisterType<SingleQuoteCache>().As<IQueryCache>().SingleInstance();
            builder.RegisterType<QueryResultStore>().As<IQueryResultStore>().SingleInstance();
            builder.RegisterType<QueryExecutorContextRegistry>().As<IQueryExecutorContextRegistry>().SingleInstance();
            builder.RegisterType<QuerySubscriptions>().SingleInstance();
            builder.RegisterType<QueryPrioritySet>().SingleInstance();
            builder.RegisterType<QuerySortBySubscriptionCountRule>().As<IQueuePriorityRule>().SingleInstance();
            builder.RegisterType<QueryEventBus>().As<IQueryEventBus>().SingleInstance();


        }
    }
}
