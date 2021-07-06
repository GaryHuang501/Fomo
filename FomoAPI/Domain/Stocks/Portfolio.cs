using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    /// <summary>
    /// User created portfolio containing the stock symbols they added.
    /// </summary>
    public class Portfolio : IEntity, IModelValidateable
    {
        public int Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Name { get;  set; }

        public DateTime DateModified { get; private set; }

        public DateTime DateCreated { get; private set; }

        public IEnumerable<PortfolioSymbol> PortfolioSymbols { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public Portfolio(int id, Guid userId, string name, DateTime dateModified, DateTime dateCreated, IEnumerable<PortfolioSymbol> portfolioSymbols)
        {
            Id = id;
            UserId = userId;
            Name = name;
            DateModified = dateModified;
            DateCreated = dateCreated;
            PortfolioSymbols = portfolioSymbols ?? new List<PortfolioSymbol>();
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
