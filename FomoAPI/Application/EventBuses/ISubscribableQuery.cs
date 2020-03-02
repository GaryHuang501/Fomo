using FomoAPI.Infrastructure.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.EventBuses
{
    /// <summary>
    /// Interface to implement if the query is subscrable to the QueryEventBus
    /// </summary>
    public interface ISubscribableQuery
    {
        QueryFunctionType FunctionType { get;}

        string Symbol { get; }

        DateTime CreateDate { get; }
    }
}
