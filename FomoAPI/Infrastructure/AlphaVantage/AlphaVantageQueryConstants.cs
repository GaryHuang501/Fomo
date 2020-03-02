using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.AlphaVantage
{
    /// <summary>
    /// Constants representing the Url Query Parameters Keys when fetching data from AlphaVantage
    /// </summary>
    public class AlphaVantageQueryKeys
    {
        public const string Function = "function";

        public const string Symbol = "symbol";

        public const string ApiKey = "apikey";

        public const string DataType = "datatype";
    }
}
