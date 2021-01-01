using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.Firebase
{
    public class FirebaseSingleQuoteEntry
    {
        public DateTime LastUpdated { get; private set; }

        public FirebaseSingleQuoteEntry(DateTime lastUpdated)
        {
            LastUpdated = lastUpdated;
        }
    }
}
