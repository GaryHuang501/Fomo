using FomoAPI.Application.EventBuses;
using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FomoAPI.Common;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <summary>
    /// Query that will be serialized as a web request to AlphaVantage for stock data.
    /// </summary>
    /// <remarks>Function is Global quote in AlphaVantage</remarks>
    public class AlphaVantageQuery
    {
        public QueryFunctionType FunctionType { get; }

        public string Ticker { get; }

        public DateTime CreateDate { get; }

        public AlphaVantageQuery(QueryFunctionType functionType, string ticker)
        {
            if (string.IsNullOrEmpty(ticker)) throw new ArgumentNullException(nameof(ticker));

            FunctionType = functionType;
            Ticker = ticker;
            CreateDate = DateTime.UtcNow;
        }

        public QueryDataType DataType { get => QueryDataType.Json; }

        public ReadOnlyDictionary<string, string> GetParameters()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { AlphaVantageQueryKeys.Function, FunctionType.Name },
                { AlphaVantageQueryKeys.Symbol, Ticker },
                { AlphaVantageQueryKeys.DataType, DataType.Name }
            });
        }


    }
}
