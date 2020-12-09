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
    public class Portfolio : IEntity
    {
        public int Id { get; private set; }

        public Guid UserId { get; private set; }

        public string Name { get; private set; }

        public DateTime DateModified { get; private set; }

        public DateTime DateCreated { get; private set; }

        public IEnumerable<PortfolioSymbol> Symbols { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public Portfolio(int id, Guid userId, string name, DateTime dateModified, DateTime dateCreated)
            : this(id, userId, name, dateModified, dateCreated, new List<PortfolioSymbol>())
        {
        }

        public Portfolio(int id, Guid userId, string name, DateTime dateModified, DateTime dateCreated, IEnumerable<PortfolioSymbol> symbols)
        {
            Id = id;
            UserId = userId;
            Name = name;
            DateModified = dateModified;
            DateCreated = dateCreated;
            Symbols = symbols;
        }
    }
}
