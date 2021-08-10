using FomoAPI.Application.DTOs;
using FomoAPIIntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIIntegrationTests.Scenarios
{
    public class AccountTests : IClassFixture<FomoApiFixture>
    {
        private readonly HttpClient _client;

        public AccountTests(FomoApiFixture webApiFactoryFixture)
        {
            webApiFactoryFixture.CreateServer(FomoApiFixture.WithNoHostedServices);
            _client = webApiFactoryFixture.GetClientNotAuth();

            var mockDbOptions = AppTestSettings.Instance.GetDbOptionsMonitor();

        }

        [Fact]
        public async Task Should_UpdateUserName()
        {
            string originalName = "TestOriginal";
            string updateName = "TestUpdated" + DateTime.UtcNow.Millisecond.ToString();

            Guid userId = await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, originalName);

            var httpMessage = new HttpRequestMessage(HttpMethod.Put, ApiPath.Account(userId.ToString()));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId.ToString());

            var userDto = new UserDTO(userId, updateName);
            httpMessage.Content = userDto.ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);

            Assert.Equal(HttpStatusCode.OK, saveResponse.StatusCode);

            var updatedUser = await saveResponse.Content.ReadAsAsync<UserDTO>();

            Assert.Equal(updateName, updatedUser.Name);
        }

        [Fact]
        public async Task Should_NotUpdateUserName_WhenLengthTooShort()
        {
            const string originalName = "TestOriginal";
            const string updateName = "1";

            Guid userId = await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString, originalName);

            var httpMessage = new HttpRequestMessage(HttpMethod.Put, ApiPath.Account(userId.ToString()));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, userId.ToString());

            var userDto = new UserDTO(userId, updateName);
            httpMessage.Content = userDto.ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);

            Assert.Equal(HttpStatusCode.BadRequest, saveResponse.StatusCode);
        }

        [Fact]
        public async Task Should_NotUpdateUser_WhenTryingToUpdateAnotherUser()
        {

            Guid userId = await TestUtil.CreateNewUser(AppTestSettings.Instance.TestDBConnectionString);
            Guid anotherUserId = Guid.NewGuid();
            var httpMessage = new HttpRequestMessage(HttpMethod.Put, ApiPath.Account(userId.ToString()));
            httpMessage.Headers.Add(TestAuthHandler.CustomUserIdHeader, anotherUserId.ToString());

            var userDto = new UserDTO(userId, "test");
            httpMessage.Content = userDto.ToJsonPayload();

            var saveResponse = await _client.SendAsync(httpMessage);

            Assert.Equal(HttpStatusCode.Forbidden, saveResponse.StatusCode);
        }
    }
}
