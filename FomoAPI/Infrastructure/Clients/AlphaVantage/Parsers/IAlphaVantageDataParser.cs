using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using System.IO;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParser<T> where T : StockData
    {
        T ParseCsv(int symbolId, StreamReader reader);

        T ParseJson(int symbolId, string json);
    }
}
