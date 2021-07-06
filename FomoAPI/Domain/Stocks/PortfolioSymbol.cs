using Dapper;
using Newtonsoft.Json;

namespace FomoAPI.Domain.Stocks
{
    public record PortfolioSymbol : IEntity, IModelValidateable
    {
        public int Id { get; init; }

        public int SymbolId { get; init; }

        public string Ticker { get; init; }

        public string ExchangeName { get; init; }

        public string FullName { get; init; }

        public bool Delisted { get; init; }

        public decimal AveragePrice { get; init; }

        public int SortOrder { get; init; }

        [JsonConstructor]
        [ExplicitConstructor]
        public PortfolioSymbol(int id, int symbolId, string ticker, string exchangeName, string fullName, decimal averagePrice, bool delisted, int sortOrder)
        {
            Id = id;
            SymbolId = symbolId;
            Ticker = ticker;
            ExchangeName = exchangeName;
            FullName = fullName;
            Delisted = delisted;
            SortOrder = sortOrder;
            AveragePrice = averagePrice;
        }

        public bool IsValid()
        {
            var validator = new PortfolioSymbolValidator();

            var result = validator.Validate(this);

            return result.IsValid;
        }
    }
}
