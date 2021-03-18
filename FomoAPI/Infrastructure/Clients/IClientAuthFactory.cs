
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients
{
    /// <summary>
    /// Creates authentication token for third party clients.
    /// </summary>
    public interface IClientAuthFactory
    {
        /// <summary>
        /// Create the access auth token so server can communicate with API.
        /// </summary>
        /// <returns>The token string</returns>
        Task<string> CreateServerAccessToken();

        /// <summary>
        /// Creates a token for client to login to comunicate with API
        /// </summary>
        /// <returns>The token string</returns>
        Task<string> CreateClientToken(string userId, IReadOnlyDictionary<string, object> claims);
    }
}
