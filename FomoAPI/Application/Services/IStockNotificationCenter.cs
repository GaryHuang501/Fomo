using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <summary>
    /// Notifies Client of stock changes
    /// </summary>
    public interface IStockNotificationCenter
    {
        Task NotifySingleQuoteChanges(IEnumerable<int> symbolIdsChanged);
    }
}
