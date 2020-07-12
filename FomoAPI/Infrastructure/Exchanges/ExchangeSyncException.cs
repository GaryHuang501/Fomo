using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Exchanges
{
    public class ExchangeSyncException : Exception
    {
        public ExchangeSyncException()
        {
        }

        public ExchangeSyncException(string message) : base(message)
        {
        }

        public ExchangeSyncException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
