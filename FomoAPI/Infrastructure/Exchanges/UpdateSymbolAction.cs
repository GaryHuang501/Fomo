using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    /// <summary>
    /// Update Action object for update symbol fields to Database.
    /// </summary>
    public class UpdateSymbolAction
    {
        public int Id { get; private set; }

        public string FullName { get; private set; }

        public int ExchangeId { get; private set; }

        public UpdateSymbolAction(int id, string fullName, int exchangeId)
        {
            Id = id;
            FullName = fullName;
            ExchangeId = exchangeId;
        }
    }
}
