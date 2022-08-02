using FomoAPI.Common;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class SymbolKey
    {
        public string Ticker { get; private set; }

        public int ExchangeId { get; private set; }

        public SymbolKey(string ticker, int exchangeId)
        {
            Ticker = ticker;
            ExchangeId = exchangeId;
        }

        public bool Equals(SymbolKey other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object b)
        {
            return
                   (b is SymbolKey other) &&
                   (ExchangeId == other.ExchangeId) &&
                   (Ticker == other.Ticker);
        }

        public override int GetHashCode()
        {
            return this.CalculateHashCodeForParams(Ticker, ExchangeId);
        }
    }
}
