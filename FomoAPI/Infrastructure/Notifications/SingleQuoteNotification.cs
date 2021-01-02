using System;

namespace FomoAPI.Infrastructure.Notifications
{
    public class SingleQuoteNotification : INotification
    {
        public DateTime LastUpdated { get; private set; }
       
        public string Key { get; private set; }

        public SingleQuoteNotification(string key, DateTime lastUpdated)
        {
            Key = key;
            LastUpdated = lastUpdated;
        }
    }
}
