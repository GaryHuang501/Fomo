using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public interface IExchangeParser
    {
        Task<IDictionary<string, DownloadedSymbol>> GetTickerToSymbolMap(StreamReader reader, string delimiter, string[] suffixBlackList);
    }
}
