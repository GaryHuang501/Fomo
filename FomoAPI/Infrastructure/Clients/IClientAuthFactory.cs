
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients
{
    /// <summary>
    /// Creates authentication token for third party clients.
    /// </summary>
    public interface IClientAuthFactory
    {
        /// <summary>
        /// Create the authentication token
        /// </summary>
        /// <returns>The token string</returns>
        Task<string> CreateAuthToken();
    }
}
