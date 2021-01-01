using FomoAPI.Infrastructure.ConfigurationOptions;
using Google.Apis.Auth.OAuth2;
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
        private readonly FirebaseOptions _firebaseOptions;

        public FireBaseDBClient(IHttpClientFactory clientFactory, IOptionsMonitor<FirebaseOptions> firebaseOptionsMonitor, IClientAuthFactory authFactory)
        {
            _clientFactory = clientFactory;
            _authFactory = authFactory;
            _firebaseOptions = firebaseOptionsMonitor.CurrentValue;
        }

        /// <summary>
        /// Upsert entry in database.
        /// </summary>
        /// <param name="path">Path to entry.</param>
        /// <param name="jsonObject">Object to insert/update entry with.</param>
        public async Task NotifyUpsert(string path, object jsonObject)
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

        private async Task<string> SetAuthTokenUrl(string uri)
        {
            if (!_firebaseOptions.AuthEnabled)
            {
                return uri;
            }

            string authToken = await _authFactory.CreateAuthToken();
            return $"{uri}?access_token={authToken}";
        }
    }
}
