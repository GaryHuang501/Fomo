using System.IO;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using FomoAPI.AutoFacModules;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("authentication.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("firebase.json", optional: false, reloadOnChange: true)
                     .AddJsonFile("validation.json", optional: false, reloadOnChange: true);

if (env.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
    builder.Logging.AddJsonConsole();
}
else if (env.EnvironmentName == "Test")
{
    const string testSecretsId = "22803d79-afbc-493c-a3f7-f05a6c451f7c";
    builder.Configuration.AddUserSecrets(testSecretsId);
}

builder.Configuration.AddEnvironmentVariables();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
{
    containerBuilder.RegisterModule(new ApiModule())
                    .RegisterModule(new ExchangeSyncModule())
                    .RegisterModule(new EventBusModule());
});

var startup = new FomoAPI.Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();
