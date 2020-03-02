
using Newtonsoft.Json;
using System;

namespace FomoAPI.Domain.Stocks
{
    public class StockMetaData
    {
        public StockMetaData(
            string information, 
            string symbol, 
            DateTime lastRefreshed, 
            string interval, 
            string outputSize, 
            string timeZone)
        {
            Information = information;
            Symbol = symbol;
            LastRefreshed = lastRefreshed;
            Interval = interval;
            OutputSize = outputSize;
            TimeZone = timeZone;
        }

        public string Information { get; private set; }

        public string Symbol { get; private set; }

        public DateTime LastRefreshed { get; private set; }

        public string Interval { get; private set; }

        public string OutputSize { get; private set; }

        public string TimeZone { get; private set; }
    }
}
