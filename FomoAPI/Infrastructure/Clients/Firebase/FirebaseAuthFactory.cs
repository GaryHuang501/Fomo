using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FomoAPI.Infrastructure.ConfigurationOptions;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FomoAPI.Infrastructure.Clients.Firebase
{
    /// <summary>
    /// Singleton factory to maintain firebase credentials for server.
    /// Will initialize with the token and periodically renew the token.
    /// </summary>
    public class FirebaseAuthFactory : IClientAuthFactory, IHostedService, IDisposable
    {
        private readonly FireBaseOptions _firebaseOptions;
        private readonly ILogger<FirebaseAuthFactory> _logger;
        private string _authToken;
        private DateTime _lastRenewTime;
        private Timer _renewTokenTimer;
        private readonly SemaphoreSlim _semaphore;
        private GoogleCredential _googleCreds;
        private FirebaseApp _firebaseApp;


        public FirebaseAuthFactory(IOptionsMonitor<FireBaseOptions> firebaseOptionsMonitor, ILogger<FirebaseAuthFactory> logger)
        {
            _firebaseOptions = firebaseOptionsMonitor.CurrentValue;
            _logger = logger;
            _semaphore = new SemaphoreSlim(1);
        }

        /// <summary>
        /// Creates a server access token if it has not been created yet.
        /// Otherwise it will reuse the token.
        /// </summary>
        /// <returns>The token string.</returns>
        public async Task<string> CreateServerAccessToken()
        {
            if (_authToken == null)
            {
                await RenewServerAccessToken();
            }

            return _authToken;
        }

        /// <summary>
        /// Creates a custom token to be returned to client to log into firebase api.
        /// </summary>
        /// <returns>The token string</returns>
        public async Task<string> CreateClientToken(Guid userId, IReadOnlyDictionary<string, object> claims)
        {
            string userIdString = userId.ToString();
            await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userIdString, claims);
            string customToken = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(userIdString);

            return customToken;
        }

        public void Dispose()
        {
            if (_renewTokenTimer != null)
            {
                _renewTokenTimer.Dispose();
            }

            _firebaseApp.Delete();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var renewTimeSpan = TimeSpan.FromMinutes(_firebaseOptions.TokenRenewalMinutes);

            _googleCreds = GoogleCredential.FromJson(_firebaseOptions.ServiceAccountCredentials);

            _firebaseApp = FirebaseApp.DefaultInstance ?? FirebaseApp.Create(new AppOptions()
            {
                Credential = _googleCreds
            });

            var claims = new Dictionary<string, object>()
            {
                { "admin", true },
            };

            if (!_firebaseOptions.AuthEnabled)
            {
                return;
            }

            await RenewServerAccessToken();

            _renewTokenTimer = new Timer(async (state) =>
            {
                try
                {
                    _logger.LogInformation("Renewing Firebase token. Last renewed:", _lastRenewTime.ToString());
                    _googleCreds = GoogleCredential.FromJson(_firebaseOptions.ServiceAccountCredentials);
                    await RenewServerAccessToken();
                }catch(Exception ex)
                {
                    _logger.LogCritical("Error renewing firebase token", ex);
                }
            }, null, renewTimeSpan, renewTimeSpan);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task RenewServerAccessToken()
        {
            bool lockAcquired = await _semaphore.WaitAsync(0);

            if(!lockAcquired)
            {
                return;
            }

            // Add the required scopes to the Google credential
            GoogleCredential scoped = _googleCreds.CreateScoped(
                                                                new List<string>
                                                                {
                                                                "https://www.googleapis.com/auth/firebase.database",
                                                                "https://www.googleapis.com/auth/userinfo.email"
                                                                });

            _authToken =  await scoped.UnderlyingCredential.GetAccessTokenForRequestAsync();
            _lastRenewTime = DateTime.UtcNow;

            _semaphore.Release();
        }

        public async Task<bool> VerifyUser(Guid userId)
        {
            try
            {
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(userId.ToString());
            }
            catch(FirebaseException)
            {
                return false;
            }

            return true;
        }

        public async Task CreateUser(Guid userId, string userName, string email)
        {
            UserRecordArgs args = new()
            {
                Email = email,
                EmailVerified = false,
                PhoneNumber = null,
                Password = Guid.NewGuid().ToString(), // Create a random password. Doesn't matter as we'll use a service account to manage users.
                DisplayName = userName,
                PhotoUrl = null,
                Disabled = false,
                Uid = userId.ToString(),           
            };

            await FirebaseAuth.DefaultInstance.CreateUserAsync(args);
        }
    }
}
