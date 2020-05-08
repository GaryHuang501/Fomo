using System;
using System.Net.Http;

namespace FomoAPIIntegrationTests
{
    public class MockHttpClientFactory : IHttpClientFactory
    {
        public string Url { get; set; }

        public HttpClient Client;
        public MockHttpClientFactory(string url)
        {
            Url = url;
        }

        public HttpClient CreateClient(string name)
        {
            Client = new HttpClient()
            {
                BaseAddress = new Uri(Url)
            };

            return Client;
        }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
