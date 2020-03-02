using FomoAPI.Domain.Stocks;

namespace FomoAPI.Infrastructure.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParserFactory
    {
        IAlphaVantageDataParser<StockSingleQuoteData> GetSingleQuoteDataParser();
    }
}