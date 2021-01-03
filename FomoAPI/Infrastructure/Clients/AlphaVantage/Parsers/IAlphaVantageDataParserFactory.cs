using FomoAPI.Domain.Stocks;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParserFactory
    {
        IAlphaVantageDataParser<SingleQuoteData> GetSingleQuoteDataParser();
    }
}