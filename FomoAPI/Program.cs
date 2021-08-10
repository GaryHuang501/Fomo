using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FomoAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                              .AddJsonFile("authentication.json", optional: false, reloadOnChange: true)
                              .AddJsonFile("firebase.json", optional: false, reloadOnChange: true)
                              .AddJsonFile("validation.json", optional: false, reloadOnChange: true);

                        if (hostingContext.HostingEnvironment.IsDevelopment())
                        {
                            config.AddUserSecrets<Program>();
                        }
                        else if(hostingContext.HostingEnvironment.EnvironmentName == "Test")
                        {
                            const string testSecretsId = "22803d79-afbc-493c-a3f7-f05a6c451f7c";
                            config.AddUserSecrets(testSecretsId);
                        }

                        config.AddEnvironmentVariables();
                    })
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
                    .ConfigureLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                    })
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
    }
}
