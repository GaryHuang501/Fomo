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

        public FirebaseAuthFactory(IOptionsMonitor<FireBaseOptions> firebaseOptionsMonitor, ILogger<FirebaseAuthFactory> logger)
        {
            _firebaseOptions = firebaseOptionsMonitor.CurrentValue;
            _logger = logger;
        }

        public async Task<string> CreateAuthToken()
        {
            if (_authToken == null)
            {
                await RenewToken();
            }

            return _authToken;
        }

        public void Dispose()
        {
            if (_renewTokenTimer != null)
            {
                _renewTokenTimer.Dispose();
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var renewTimeSpan = TimeSpan.FromMinutes(_firebaseOptions.TokenRenewalMinutes);

            if (!_firebaseOptions.AuthEnabled)
            {
                return;
            }

            await RenewToken();

            _renewTokenTimer = new Timer(async (state) =>
            {
                try
                {
                    _logger.LogInformation("Renewing Firebase token. Last renewed:", _lastRenewTime.ToString());
                    await RenewToken();
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


        private async Task RenewToken()
        {

            // Authenticate a Google credential with the service account
            GoogleCredential googleCred = GoogleCredential.FromJson(_firebaseOptions.ServiceAccountCredentials);

            // Add the required scopes to the Google credential
            GoogleCredential scoped = googleCred.CreateScoped(
                                                                new List<string>
                                                                {
                                                                "https://www.googleapis.com/auth/firebase.database",
                                                                "https://www.googleapis.com/auth/userinfo.email"
                                                                });

            // Use the Google credential to generate an access token
            var oidcOptions = OidcTokenOptions.FromTargetAudience("https://localhost");
            OidcToken token = await scoped.GetOidcTokenAsync(oidcOptions);
            _authToken = await token.GetAccessTokenAsync();
            _lastRenewTime = DateTime.UtcNow;
        }
    }
}
