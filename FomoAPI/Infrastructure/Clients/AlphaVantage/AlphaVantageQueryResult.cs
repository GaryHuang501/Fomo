using FomoAPI.Application.EventBuses;
using FomoAPI.Domain.Stocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    /// <summary>
    /// Wrapper class holding the Query Data from AlphaVantage
    /// Or holds the error when the query failed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AlphaVantageQueryResult<T>: ISubscriptionQueryResult where T: IQueryableData
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
