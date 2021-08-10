using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Domain.Login;
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

            services.AddOptions<FinnHubOptions>()
                    .Bind(config.GetSection("FinnHub"))
                    .ValidateDataAnnotations();

            services.AddOptions<AccountsOptions>()
                    .Bind(config.GetSection("Accounts"))
                    .ValidateDataAnnotations();

            services.AddOptions<DbOptions>()
                    .Bind(config.GetSection("Database"))
                    .ValidateDataAnnotations();

            services.AddOptions<PortfolioStocksUpdateOptions>()
                    .Bind(config.GetSection("PortfolioStockUpdate"))
                    .ValidateDataAnnotations();

            services.AddOptions<EventBusOptions>()
                    .Bind(config.GetSection("EventBus"))
                    .ValidateDataAnnotations();

            services.AddOptions<ExchangeSyncOptions>()
                    .Bind(config.GetSection("ExchangeSync"))
                    .ValidateDataAnnotations();

            services.AddOptions<FireBaseOptions>()
                    .Bind(config.GetSection("FireBase"))
                    .ValidateDataAnnotations();

            services.AddOptions<CacheOptions>("SingleQuoteCache")
                    .Bind(config.GetSection("SingleQuoteCache"))
                    .ValidateDataAnnotations();

            services.AddOptions<CacheOptions>("StockSearchCache")
                    .Bind(config.GetSection("StockSearchCache"))
                    .ValidateDataAnnotations();

            services.AddOptions<UserValidationOptions>()
                    .Bind(config.GetSection("Validation:Users"))
                    .ValidateDataAnnotations();
        }
    }
}
