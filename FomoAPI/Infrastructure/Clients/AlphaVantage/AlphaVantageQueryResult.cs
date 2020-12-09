using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks;
using System;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <summary>
    /// Wrapper class holding the Query Data from AlphaVantage
    /// Or holds the error when the query failed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlphaVantageQueryResult<T> where T : StockData
    {
        public DateTime CreateDateUtc { get; private set; }

        public T Data { get; private set; }

        public bool HasError => ErrorMessage != null;

        public string ErrorMessage { get;  private set; }

        public AlphaVantageQueryResult(T data)
        {
            Data = data;
            CreateDateUtc = DateTime.UtcNow;
        }

        public AlphaVantageQueryResult(string error)
        {
            ErrorMessage = error;
        }
    }
}
