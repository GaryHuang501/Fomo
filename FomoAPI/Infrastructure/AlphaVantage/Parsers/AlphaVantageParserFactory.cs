using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.AlphaVantage.Parsers
{
    public class AlphaVantageParserFactory: IAlphaVantageDataParserFactory
    {
        public IAlphaVantageDataParser<StockSingleQuoteData> GetSingleQuoteDataParser()
        {
            return new StockSingleQuoteDataCsvParser();
        }
    }
}
