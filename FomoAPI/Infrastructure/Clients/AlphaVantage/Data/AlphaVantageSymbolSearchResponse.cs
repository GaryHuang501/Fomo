﻿namespace FomoAPI.Infrastructure.Clients.AlphaVantage.Data
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
