using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class ExchangeSyncHistory
    {
        public int Id { get; set; }

        public string ActionName { get; set; }

        public string  Message { get; set; }

        public int SymbolsChanged { get; set; }

        public string Error { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
