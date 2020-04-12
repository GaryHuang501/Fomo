using Autofac;
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
            builder.RegisterType<PortfolioRepository>().As<IPortfolioRepository>().InstancePerRequest();
        }
    }
}
