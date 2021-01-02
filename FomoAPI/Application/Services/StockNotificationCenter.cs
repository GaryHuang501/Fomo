using FomoAPI.Infrastructure.Clients;
using FomoAPI.Infrastructure.Clients.Firebase;
using FomoAPI.Infrastructure.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Application.Services
{
    /// <inheritdoc cref="IStockNotificationCenter"/>
    public class StockNotificationCenter : IStockNotificationCenter
    {
        private readonly INotificationClient _client;

        public StockNotificationCenter(INotificationClient client)
        {
            _client = client;
        }

        public async Task NotifySingleQuoteChanges(IEnumerable<int> symbolIdsChanged)
        {
            foreach(var id in symbolIdsChanged)
            {
                var notification = new SingleQuoteNotification($"singleQuoteData/{id}", DateTime.UtcNow);
                await _client.NotifyChanges(notification);
            }
        }
    }
}
