using Autofac;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Application.Stores;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers;
using Microsoft.Extensions.Hosting;

namespace FomoAPI.AutoFacModules
{
    public class EventBusModule: Module
    {  
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SingleQuoteCache>().SingleInstance();
            builder.RegisterType<QueryContextFactory>().As<IQueryContextFactory>().SingleInstance();
            builder.RegisterType<QuerySubscriptions>().SingleInstance();
            builder.RegisterType<QueryQueue>().SingleInstance();
            builder.RegisterType<QuerySubscriptionCountRule>().As<IQueuePriorityRule>().SingleInstance();
            builder.RegisterType<QueryEventBus>().As<IQueryEventBus>().SingleInstance();
            builder.RegisterType<QueryEventBusTimedHostedService>().As<IHostedService>().SingleInstance();

            builder.RegisterType<SingleQuoteContext>().InstancePerDependency();

            builder.RegisterType<AlphaVantageParserFactory>().As<IAlphaVantageDataParserFactory>().InstancePerDependency();
            builder.RegisterType<AlphaVantageClient>().As<IStockClient>().InstancePerDependency();

        }
    }
}
