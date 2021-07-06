using FomoAPI.Domain.Stocks;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace FomoAPI.Application.Patches
{
    /// <summary>
    /// Extensions on <see cref="JsonPatchDocument"/> for Portfolio Objects to apply the patches
    /// and return a new instance. This is to handle immutable objects, which ApplyTo doesn't support.
    /// </summary>
    public static class PortfolioJsonPatchDocumentExtensions
    {   
        public static Portfolio CopyTo(this JsonPatchDocument<Portfolio> patchDocument, Portfolio portfolio)
        {
            string newName = string.Empty;

            foreach(var patch in patchDocument.Operations)
            {
                if(IsPathMatch(patch.path, nameof(portfolio.Name))
                    && (IsEditPatchOp(patch.op)))
                {
                    newName = patch.value.ToString();
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported patch for Portfolio: {JsonConvert.SerializeObject(patch)}");
                }
            }

            var newPortfolio = new Portfolio
                (
                    id: portfolio.Id,
                    userId: portfolio.UserId,
                    name: newName,
                    dateModified: portfolio.DateModified,
                    dateCreated: portfolio.DateCreated,
                    portfolioSymbols: portfolio.PortfolioSymbols.ToList()
                );

            return newPortfolio;
        }

        public static PortfolioSymbol CopyTo(this JsonPatchDocument<PortfolioSymbol> patchDocument, PortfolioSymbol portfolioSymbol)
        {
            decimal newAveragePrice = 0;

            foreach (var patch in patchDocument.Operations)
            {
                if (IsPathMatch(patch.path, nameof(portfolioSymbol.AveragePrice))
                    && (IsEditPatchOp(patch.op)))
                {
                    newAveragePrice = decimal.Parse(patch.value.ToString());
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported patch for Portfolio: {JsonConvert.SerializeObject(patch)}");
                }
            }

            var newPortfolioSymbol = new PortfolioSymbol
                (
                    id: portfolioSymbol.Id,
                    symbolId: portfolioSymbol.SymbolId,
                    ticker: portfolioSymbol.Ticker,
                    exchangeName: portfolioSymbol.ExchangeName,
                    fullName: portfolioSymbol.FullName,
                    averagePrice: newAveragePrice,
                    delisted: portfolioSymbol.Delisted,
                    sortOrder: portfolioSymbol.SortOrder
                );

            return newPortfolioSymbol;
        }

        private static bool IsEditPatchOp(string op)
        {
            return op.Equals(HttpPatchOp.Add, StringComparison.InvariantCultureIgnoreCase) 
                || op.Equals(HttpPatchOp.Replace, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsPathMatch(string path, string propertyName)
        {
            return path.Trim('/', '\\').Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
