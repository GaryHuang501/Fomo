using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace FomoAPIIntegrationTests
{
    public static class ApiTestExtensions
    {
        public static StringContent ToJsonPayload(this object obj)
        {
            var payload = JsonConvert.SerializeObject(obj);
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }
    }
}
