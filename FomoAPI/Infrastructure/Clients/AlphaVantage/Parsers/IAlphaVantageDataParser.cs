using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Parsers
{
    public interface IAlphaVantageDataParser<T> where T: IQueryableData
    {
        T ParseData(StreamReader reader);
    }
}
