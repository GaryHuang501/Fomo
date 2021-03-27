using Autofac;
using FomoAPI.Application.Exchanges;
using FomoAPI.Infrastructure;
using FomoAPI.Infrastructure.Exchanges;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.AutoFacModules
{
    public class ExchangeSyncModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExchangeSyncHostedService>().As<IHostedService>().SingleInstance();

            builder.RegisterType<FtpClient>().As<IFtpClient>().InstancePerDependency();
            builder.RegisterType<NasdaqParser>().As<IExchangeParser>().InstancePerDependency();
            builder.RegisterType<ExchangeClient>().As<IExchangeClient>().InstancePerDependency();
            builder.RegisterType<ExchangeSyncChangesetsFactory>().As<IExchangeSyncChangesetsFactory>().InstancePerDependency();
            builder.RegisterType<ExchangeSyncRepository>().As<IExchangeSyncRepository>().InstancePerDependency();
            builder.RegisterType<ExchangeSync>().As<IExchangeSync>().InstancePerDependency();
        }
    }
}
