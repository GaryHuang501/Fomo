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
    public record Portfolio : IEntity, IModelValidateable
    {
        public int Id { get; init; }

        public Guid UserId { get; init; }

        public string Name { get;  set; }

        public DateTime DateModified { get; init; }

        public DateTime DateCreated { get; init; }

        public IEnumerable<PortfolioSymbol> PortfolioSymbols { get; init; }

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
