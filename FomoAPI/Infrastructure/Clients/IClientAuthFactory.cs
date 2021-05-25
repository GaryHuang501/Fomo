
using System;
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
        Task<string> CreateClientToken(Guid userId, IReadOnlyDictionary<string, object> claims);

        /// <summary>
        /// Creates new fire base user
        /// </summary>
        /// <param name="userId">UserID to be used as Firebase UID.</param>
        /// <param name="email">Email Address</param>
        Task CreateUser(Guid userId, string userName, string email);

        /// <summary>
        /// Verifies it is a valid UID that eixsts in firebase.
        /// </summary>
        /// <param name="userId">The UserId to verify.</param>
        /// <returns>True if valid.</returns>
        Task<bool> VerifyUser(Guid userId);
    }
}
