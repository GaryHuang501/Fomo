using FomoAPI.Domain.Stocks;
using FomoAPI.Domain.Stocks.Queries;
using System.IO;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParser<T> where T : StockData
    {
        T ParseData(StreamReader reader);
    }
}
