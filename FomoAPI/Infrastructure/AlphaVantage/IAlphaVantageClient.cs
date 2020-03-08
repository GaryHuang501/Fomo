using FomoAPI.Domain.Stocks;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.AlphaVantage
{
    /// <summary>
    /// HTTP Client wrapper class to fetch data from Alpha Vantage API
    /// </summary>
    public interface IAlphaVantageClient
    {
        Task<AlphaVantageQueryResult<StockSingleQuoteData>> GetSingleQuoteData(AlphaVantageSingleQuoteQuery query);
    }
}