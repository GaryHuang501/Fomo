using Autofac;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Application.Queries;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Stocks;
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
            builder.RegisterType<PortfolioStocksUpdateHostedService>().As<IHostedService>().SingleInstance();
            builder.RegisterType<PortfolioStocksUpdateQuery>().As<IPortfolioStocksUpdateQuery>().SingleInstance();

            builder.RegisterType<SingleQuoteContext>().InstancePerDependency();
            builder.RegisterType<NasdaqMarketHours>().As<IMarketHours>().InstancePerDependency();

        }
    }
}
