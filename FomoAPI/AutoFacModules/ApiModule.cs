using Autofac;
using FomoAPI.Application.DTOs;
using FomoAPI.Application.Services;
using FomoAPI.Application.Stores;
using FomoAPI.Controllers.Authorization;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.Clients;
using FomoAPI.Infrastructure.Clients.AlphaVantage;
using FomoAPI.Infrastructure.Clients.Firebase;
using FomoAPI.Infrastructure.Notifications;
using FomoAPI.Infrastructure.Repositories;
using FomoAPI.Infrastructure.Stocks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
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
            builder.RegisterType<PortfolioOwnerHandler>().As<IAuthorizationHandler>();

            builder.RegisterType<FirebaseAuthFactory>().As<IHostedService>().As<IClientAuthFactory>().SingleInstance();
            builder.RegisterType<StockSearchCache>().SingleInstance();
            builder.RegisterType<SingleQuoteCache>().SingleInstance();

            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerLifetimeScope();
            builder.RegisterType<StockDataRepository>().As<IStockDataRepository>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolRepository>().As<ISymbolRepository>().InstancePerLifetimeScope();
            builder.RegisterType<VoteRepository>().As<IVoteRepository>().InstancePerLifetimeScope();

            builder.RegisterType<AlphaVantageClient>().As<IStockClient>().InstancePerDependency();
            builder.RegisterType<FireBaseDBClient>().As<INotificationClient>().InstancePerLifetimeScope();
            builder.RegisterType<StockDataService>().As<IStockDataService>().InstancePerLifetimeScope();
            builder.RegisterType<StockNotificationCenter>().As<IStockNotificationCenter>().InstancePerLifetimeScope();
            builder.RegisterType<SymbolSearchService>().As<ISymbolSearchService>().InstancePerLifetimeScope();
        }
    }
}
