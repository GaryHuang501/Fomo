using FomoAPI.Infrastructure.ConfigurationOptions;
using FomoAPI.Infrastructure.Notifications;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.Firebase
{
    /// <summary>
    /// Firebase realtime database rest client taht will trigger connected clients to update.
    /// </summary>
    public class FireBaseDBClient : INotificationClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IClientAuthFactory _authFactory;
        private readonly FireBaseOptions _firebaseOptions;

        public FireBaseDBClient(IHttpClientFactory clientFactory, IOptionsMonitor<FireBaseOptions> firebaseOptionsMonitor, IClientAuthFactory authFactory)
        {
            _clientFactory = clientFactory;
            _authFactory = authFactory;
            _firebaseOptions = firebaseOptionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Pushes notification to clients for upsert change
        /// </summary>
        /// <param name="notification"><see cref="INotification"/> to push to clients.</param>
        public async Task NotifyChanges(INotification notification)
        {
            await Upsert(notification.Key, notification);
        }

        /// <summary>
        /// Update or insert change to firebase
        /// </summary>
        /// <param name="path">Path to entry.</param>
        /// <param name="jsonObject">Object to upsert.</param>
        public async Task Upsert(string path, object jsonObject)
        {
            HttpClient client = _clientFactory.CreateClient(_firebaseOptions.ClientName);

            string uri = $"{_firebaseOptions.DatabaseUrl}/{path}.json";
            string authorizedUri = await SetAuthTokenUrl(uri);

            var content = new StringContent(JsonConvert.SerializeObject(jsonObject));
            var request = new HttpRequestMessage(HttpMethod.Put, authorizedUri);
            request.Content = content;

            HttpResponseMessage response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Get entry from database.
        /// </summary>
        /// <typeparam name="T">Type of object to return.</typeparam>
        /// <param name="path">Path to entry to fetch</param>
        /// <returns>Firebase entry from <paramref name="path"/> of type <typeparamref name="T"/></returns>
        public async Task<T> Get<T>(string path)
        {
            HttpClient client = _clientFactory.CreateClient(_firebaseOptions.ClientName);

            string uri = $"{_firebaseOptions.DatabaseUrl}/{path}.json";
            string authorizedUri = await SetAuthTokenUrl(uri);

            var request = new HttpRequestMessage(HttpMethod.Get, authorizedUri);

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsAsync<T>();
        }

        private async Task<string> SetAuthTokenUrl(string uri)
        {
            if (!_firebaseOptions.AuthEnabled)
            {
                return uri;
            }

            string authToken = await _authFactory.CreateServerAccessToken();
            return $"{uri}?access_token={authToken}";
        }
    }
}
