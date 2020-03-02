using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks;
using FomoAPI.Infrastructure.AlphaVantage.Parsers;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.AlphaVantage
{
    /// <summary>
    /// Query object for different function types to run against AlphaVantage
    /// </summary>
    public interface IAlphaVantageQuery : ISubscribableQuery
    {
        QueryDataType DataType { get; }

        /// <summary>
        /// Get the Http Url Query parameters
        /// </summary>
        /// <returns>Query Parameters as a dictionary</returns>
        ReadOnlyDictionary<string, string> GetParameters();
    }
}
