using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FomoAPIIntegrationTests
{
    public class ApiPath
    {
        public static string Account(string id = null) => $"api/Accounts/{id}";

        public static string LeaderBoard(int limit) => $"api/LeaderBoard/?limit={limit}";

        public static string MembersData(int limit, int offset) => $"api/Members/?limit={limit}&offset={offset}";

        public static string Portfolio(int? portfolioId = null) => $"api/Portfolios/{portfolioId}";

        public static string PortfolioSymbols(int portfolioId, int? portfolioSymbolId = null) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/{portfolioSymbolId}";

        public static string PortfolioSymbolsReorder(int portfolioId) => $"api/Portfolios/{portfolioId}/PortfolioSymbols/sortorder";

        public static string SymbolSearch(string ticker, int limit) => $"api/Symbols/?keywords={ticker}&limit={limit}";

        public static string GetSingleQuoteData(int[] symbolIds) {

            return $"api/SingleQuoteData?{GetMultipleIdsQuery("sids", symbolIds)}";
        }

        public static string Votes => "api/votes";

        public static string GetVotes(int[] symbolIds)
        {
            return $"{Votes}?{GetMultipleIdsQuery("sids", symbolIds)}";
        }

        private static string GetMultipleIdsQuery<T>(string key, T[] values)
        {
            StringBuilder builder = new StringBuilder();

            if (values.Length > 0)
            {
                builder.Append($"{key}={values[0]}");
            }

            for (var i = 1; i < values.Length; i++)
            {
                builder.Append($"&{key}={values[i]}");
            }

            return builder.ToString();
        }
    }
}
