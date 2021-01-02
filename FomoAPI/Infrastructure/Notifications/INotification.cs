using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Notifications
{
    /// <summary>
    /// Data object to push notification to clients.
    /// </summary>
    public interface INotification
    {
       string Key { get; }

       DateTime LastUpdated { get; }
    }
}
