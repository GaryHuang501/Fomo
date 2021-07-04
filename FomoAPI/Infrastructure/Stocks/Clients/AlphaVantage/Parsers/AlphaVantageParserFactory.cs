﻿using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage.Parsers
{
    public class AlphaVantageParserFactory: IAlphaVantageDataParserFactory
    {
        public IAlphaVantageDataParser<SingleQuoteData> GetSingleQuoteDataParser()
        {
            return new SingleQuoteParser();
        }
    }
}
