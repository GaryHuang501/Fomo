using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FomoAPI.Setup
{
    public static class OptionsSetup
    {
        public static void RegisterOptions(IServiceCollection services, IConfiguration config)
        {
            services.AddOptions();

            services.AddOptions<AlphaVantageOptions>()
                    .Bind(config.GetSection("AlphaVantage"))
                    .ValidateDataAnnotations();

            services.AddOptions<EventBusOptions>()
                    .Bind(config.GetSection("EventBus"))
                    .ValidateDataAnnotations();

            services.AddOptions<CacheOptions>("SingleQuoteCache")
                    .Bind(config.GetSection("SingleQuoteCache"))
                    .ValidateDataAnnotations();

            services.AddOptions<CacheOptions>("StockSearchCache")
                    .Bind(config.GetSection("StockSearchCache"))
                    .ValidateDataAnnotations();

            services.AddOptions<DbOptions>()
                    .Bind(config.GetSection("Database"))
                    .ValidateDataAnnotations();
        }
    }
}
