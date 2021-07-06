using Dapper;
using Newtonsoft.Json;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// Represents a stock's info
    /// </summary>
    public record Symbol : IEntity
    {
        public int Id { get; init; }

        public string Ticker { get; init; }

        public string ExchangeName { get; init; }

        public int ExchangeId { get; init; }

        public string FullName { get; init; }

        public bool Delisted { get; init; }

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
