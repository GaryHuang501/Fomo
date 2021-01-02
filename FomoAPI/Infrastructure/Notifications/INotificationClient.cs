using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Notifications
{
    /// <summary>
    /// Third party client to notifier listeners for stock data
    /// </summary>
    public interface INotificationClient
    {
        /// <summary>
        /// Notify users of changes.
        /// </summary>
        /// <param name="Notification"><see cref="INotification"/> to publish.</param>
        Task NotifyChanges(INotification notifcation);
    }
}
