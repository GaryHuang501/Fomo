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
    /// Query class for Single Quote Data (Function is Global quote in AlphaVantage)
    /// </summary>
    public class AlphaVantageSingleQuoteQuery : AbstractSubscribableQuery, IAlphaVantageQuery
    {
        public QueryDataType DataType { get => QueryDataType.Csv; }

        public ReadOnlyDictionary<string, string> GetParameters()
        {
            return new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
            {
                { AlphaVantageQueryKeys.Function, FunctionType.QueryFunctionName },
                { AlphaVantageQueryKeys.Symbol, Symbol },
                { AlphaVantageQueryKeys.DataType, DataType.Name }
            });
        }

        public AlphaVantageSingleQuoteQuery(string symbol) 
            : base(QueryFunctionType.SingleQuote, symbol)
        {
        }
    }
}
