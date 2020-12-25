using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomoAPIIntegrationTests
{
    public class ApiPath
    {
        public static string PortfolioPath(int? portfolioId = null) => $"api/Portfolios/{portfolioId}";

        public static string PortfolioSymbolsPath(int portfolioId, int? portfolioSymbolId = null) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/{portfolioSymbolId}";

        public static string PortfolioSymbolsReorderPath(int portfolioId) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/reorder";

        public static string SymbolSearchPath(string ticker, int limit) => $"api/Symbols/?keywords={ticker}&limit={limit}";

        public static string GetSingleQuoteDataPath(int[] symbolIds) {

            StringBuilder builder = new StringBuilder();

            if(symbolIds.Length > 0)
            {
                builder.Append($"symbolIds={symbolIds[0]}");
            }

            for (var i = 1; i < symbolIds.Length; i++)
            {
                builder.Append($"&symbolIds={symbolIds[i]}");
            }

            return $"api/Symbols/singleQuoteData?{builder.ToString()}";
        }

    }
}
