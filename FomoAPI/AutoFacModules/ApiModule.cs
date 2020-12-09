using Autofac;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using FomoAPI.Application.Stores;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
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
            builder.RegisterType<StockSearchCache>().SingleInstance();
            builder.RegisterType<SingleQuoteCache>().SingleInstance();

            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerLifetimeScope();
            builder.RegisterType<StockDataRepository>().As<IStockDataRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().InstancePerLifetimeScope();

            builder.RegisterType<AlphaVantageClient>().As<IStockClient>().InstancePerDependency();
            builder.RegisterType<StockDataService>().As<IStockDataService>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolSearchService>().As<ISymbolSearchService>().InstancePerLifetimeScope();
        }
    }
}
