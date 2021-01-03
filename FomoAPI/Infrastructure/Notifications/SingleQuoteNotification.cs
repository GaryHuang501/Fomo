using System;

namespace FomoAPI.Infrastructure.Notifications
{
    public class SingleQuoteNotification : INotification
    {
        public const string Name = "SingleQuoteData";

        public DateTime LastUpdated { get; private set; }
       
        public string Key { get; private set; }

        public SingleQuoteNotification(int symbolId, DateTime lastUpdated)
        {
            Key = $"{Name}/{symbolId}";
            LastUpdated = lastUpdated;
        }
    }
}
