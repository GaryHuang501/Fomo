using Dapper;
using Newtonsoft.Json;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Represents the info on a stock.
    /// </summary>
    public class Symbol : IEntity
    {
        public int Id { get; private set; }

        public string Ticker { get; private set; }

        public string ExchangeName { get; private set; }

        public int ExchangeId { get; private set; }

        public string FullName { get; private set; }

        public bool Delisted { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public Symbol(int id, string ticker, string exchangeName, int exchangeId, string fullName, bool delisted)
        {
            Id = id;
            Ticker = ticker;
            ExchangeName = exchangeName;
            ExchangeId = exchangeId;
            FullName = fullName;
            Delisted = delisted;
        }
    }
}
