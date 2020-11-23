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
            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().InstancePerLifetimeScope();
            builder.RegisterType<AlphaVantageClient>().As<IStockClient>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolSearchService>().As<ISymbolSearchService>().InstancePerLifetimeScope();

            builder.RegisterType<StockSearchCache>().As<ResultCache<string, IEnumerable<SymbolSearchResultDTO>>>().SingleInstance();
        }
    }
}
