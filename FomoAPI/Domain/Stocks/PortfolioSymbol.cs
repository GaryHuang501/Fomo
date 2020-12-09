using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Domain.Stocks
{
    public class PortfolioSymbol : IEntity
    {
        public int Id { get; private set; }

        public int SymbolId { get; private set; }

        public string Ticker { get; private set; }

        public string ExchangeName { get; private set; }

        public string FullName { get; private set; }

        public bool Delisted { get; private set; }

        public int SortOrder { get; private set; }

        [JsonConstructor]
        [ExplicitConstructor]
        public PortfolioSymbol(int id, int symbolId, string ticker, string exchangeName, string fullName, bool delisted, int sortOrder)
        {
            Id = id;
            SymbolId = symbolId;
            Ticker = ticker;
            ExchangeName = exchangeName;
            FullName = fullName;
            Delisted = delisted;
            SortOrder = sortOrder;
        }
    }
}
