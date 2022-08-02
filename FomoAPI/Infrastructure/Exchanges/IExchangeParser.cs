using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Parser for the downloaded symbol data from exchanges.
    /// </summary>
    public interface IExchangeParser
    {
        /// <summary>
        /// Create a dictionary that maps the stock ticker and exchange, <see cref="SymbolKey"/>, to downloaded symbol data.
        /// </summary>
        /// <param name="reader">The stream reader containing the download data</param>
        /// <param name="delimiter">Delimiter for the file columns</param>
        /// <param name="suffixBlackList">Will remove the list suffixes and any characters after it.</param>
        /// <returns><see cref="IDictionary{SymbolKey, DownloadedSymbol}"/></returns>
        Task<IDictionary<SymbolKey, DownloadedSymbol>> GetSymbolMap(StreamReader reader, string delimiter, string[] suffixBlackList);
    }
}
