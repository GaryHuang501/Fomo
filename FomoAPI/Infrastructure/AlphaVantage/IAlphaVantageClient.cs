using FomoAPI.Domain.Stocks;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.AlphaVantage
{
    public interface IAlphaVantageClient
    {
        Task<AlphaVantageQueryResult<StockSingleQuoteData>> GetSingleQuoteData(AlphaVantageSingleQuoteQuery query);
    }
}