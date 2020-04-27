using Autofac;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.AutoFacModules
{
    public class ApiModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().InstancePerLifetimeScope();
        }
    }
}
