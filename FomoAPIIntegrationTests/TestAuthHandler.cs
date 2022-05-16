using FomoAPI.Controllers.Authorization;
using FomoAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FomoAPIIntegrationTests
{
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string CustomUserIdHeader = "CustomUserId";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, 
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string userId;

            if (Request.Headers.ContainsKey(CustomUserIdHeader))
            {
                userId = Request.Headers[CustomUserIdHeader].ToString();
            }
            else
            {
                userId = AppTestSettings.Instance.DefaultUserID.ToString();
            }

            if (!TryGetPortfolioId(out string portfolioId))
            {
                portfolioId = "0";
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(FomoClaimTypes.PortfolioId, portfolioId)
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            var result = AuthenticateResult.Success(ticket);

            return Task.FromResult(result);
        }

        private bool TryGetPortfolioId(out string portfolioId)
        {
            portfolioId = null;

            if (Request.RouteValues["controller"] == null)
            {
                return false;
            }

            string controllerName = Request.RouteValues["controller"].ToString();
            string[] validControllers = new string[]
            {
                "Portfolios",
                "PortfolioSymbols"
            };

            if (!validControllers.Any(c => c.ToLower() == controllerName.ToLower()))
            {
                return false;
            }

            string[] validIdProperties = new string[]
            {
                "id",
                "portfolioId"
            };

            foreach(var idProp in validIdProperties)
            {
                if (Request.RouteValues.ContainsKey(idProp))
                {
                    portfolioId = Request.RouteValues[idProp].ToString(); ;
                    return true;
                }
            }

            return false;
        }
    }
}
