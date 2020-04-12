using Autofac;
using FomoAPI.Application.EventBuses;
using FomoAPI.Application.EventBuses.QueryExecutorContexts;
using FomoAPI.Application.EventBuses.QueuePriorityRules;
using FomoAPI.Application.Stores;
using FomoAPI.Infrastructure.AlphaVantage;
using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using Microsoft.Extensions.Hosting;

namespace FomoAPI.AutoFacModules
{
    public class EventBusModule: Module
    {  
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<QueryEventBusTimedHostedService>().As<IHostedService>().SingleInstance();

            builder.RegisterType<SingleQuoteCache>().As<IQueryCache>().SingleInstance();
            builder.RegisterType<QueryResultStore>().As<IQueryResultStore>().SingleInstance();
            builder.RegisterType<QueryExecutorContextRegistry>().As<IQueryExecutorContextRegistry>().SingleInstance();
            builder.RegisterType<QuerySubscriptions>().SingleInstance();
            builder.RegisterType<QueryPrioritySet>().SingleInstance();
            builder.RegisterType<QuerySortBySubscriptionCountRule>().As<IQueuePriorityRule>().SingleInstance();
            builder.RegisterType<QueryEventBus>().As<IQueryEventBus>().SingleInstance();

            builder.RegisterType<AlphaVantageSingleQuoteQueryExecutorContext>();
            builder.RegisterType<AlphaVantageParserFactory>().As<IAlphaVantageDataParserFactory>();
            builder.RegisterType<AlphaVantageClient>().As<IAlphaVantageClient>();
        }
    }
}
