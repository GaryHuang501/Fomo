using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Enums
{
    /// <summary>
    /// Enums for Query URL DataType parameters to determine return type for data from AlphaVantage Query
    /// </summary>
    public class QueryDataType
    {
        public readonly static QueryDataType Csv = new QueryDataType("csv");
        public readonly static QueryDataType Json = new QueryDataType("json");

        public string Name { get; private set; }

        public QueryDataType(string name)
        {
            Name = name;
        }

    }
}
