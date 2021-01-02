using FomoAPI.Infrastructure.Clients.Firebase;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.ContractTests
{
    public class FireBaseContractTests
    {
        private readonly FireBaseDBClient _client;

        private class Test
        {
            public Guid Id { get; set; }
        }

        public FireBaseContractTests()
        {
            var mockHttpFactory = new MockHttpClientFactory(AppTestSettings.Instance.FireBaseOptions.DatabaseUrl);
            var logger = new Mock<ILogger<FirebaseAuthFactory>>();
            var optionsAccessor = new Mock<IOptionsMonitor<FireBaseOptions>>();
            optionsAccessor.Setup(o => o.CurrentValue).Returns(AppTestSettings.Instance.FireBaseOptions);

            var authFactory = new FirebaseAuthFactory(optionsAccessor.Object, logger.Object);
            _client = new FireBaseDBClient(mockHttpFactory, optionsAccessor.Object, authFactory);
        }

        [Fact]
        public async Task Should_UpdateAndGet_DataFromFirebase()
        {
            var guid = Guid.NewGuid();
            await _client.Upsert($"test/{guid}", new  Test{ Id = guid });

            var get = await _client.Get<Test>($"test/{guid}");

            Assert.Equal(guid, get.Id);
        }
    }
}
