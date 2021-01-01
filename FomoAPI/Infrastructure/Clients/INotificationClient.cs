using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients
{
    /// <summary>
    /// Third party client to notifier listeners for stock data
    /// </summary>
    public interface INotificationClient
    {
        /// <summary>
        /// Notify users of an insert or update change to stock data
        /// </summary>
        /// <param name="path">Notification Path</param>
        /// <param name="args">Arguments to upsert.</param>
        Task NotifyUpsert(string path, object args);
    }
}
