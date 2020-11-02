namespace FomoAPI.Infrastructure.Clients.AlphaVantage
{
    public class AlphaVantageSymbolSearchResponse
    {
        public AlphaVantageSymbolMatch[] BestMatches { get; private set; }

        public AlphaVantageSymbolSearchResponse(AlphaVantageSymbolMatch[] bestMatches)
        {
            BestMatches = bestMatches;
        }
    }
}
