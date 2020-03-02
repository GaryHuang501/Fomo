using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class StockSingleQuoteData : IQueryableData
    {
        public string Symbol { get; private set; }

        public decimal High { get; private set; }

        public decimal Low { get; private set; }

        public decimal Open { get; private set; }

        public decimal PreviousClose { get; private set; }

        public decimal Change { get; private set; }

        public string ChangePercent { get; private set; }

        public decimal Price { get; private set; }

        public DateTime LastTradingDay { get; private set; }

        public long Volume { get; private set; }

        public StockSingleQuoteData(
            string symbol,
            decimal high,
            decimal low,
            decimal open,
            decimal price,
            decimal previousClose,
            long volume,
            decimal change,
            string changePercent,
            DateTime lastTradingDay)
        {
            Symbol = symbol;
            High = high;
            Low = low;
            Open = open;
            Price = price;
            Volume = volume;
            PreviousClose = previousClose;
            Change = change;
            ChangePercent = changePercent;
            LastTradingDay = lastTradingDay;
        }
    }
}
