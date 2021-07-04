using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks;
using System;

namespace FomoAPI.Infrastructure.Stocks.Clients.AlphaVantage
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

        public bool HasError { get; private set; }

        public string ErrorMessage { get;  private set; }

        public AlphaVantageQueryResult(T data)
        {
            Data = data;
            CreateDateUtc = DateTime.UtcNow;
        }

        public AlphaVantageQueryResult(AlphaVantageQueryError error)
        {
            ErrorMessage = error.ErrorMessage ?? error.Note;
            HasError = true;
        }

        public AlphaVantageQueryResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            HasError = true;
        }
    }
}
