using FomoAPI.Domain.Stocks;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParserFactory
    {
        IAlphaVantageDataParser<SingleQuoteData> GetSingleQuoteDataParser();
    }
}