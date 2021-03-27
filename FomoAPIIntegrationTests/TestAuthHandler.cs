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

        private readonly IPortfolioRepository _portfolioRepository;

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            IPortfolioRepository portfolioRepository,
            ILoggerFactory logger, 
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _portfolioRepository = portfolioRepository;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var userId = AppTestSettings.Instance.TestUserId.ToString();

            if (Request.Headers.ContainsKey(CustomUserIdHeader))
            {
                userId = Request.Headers[CustomUserIdHeader].ToString();
            }

            if (Request.Headers.ContainsKey(CustomUserIdHeader))
            {
                userId = Request.Headers[CustomUserIdHeader].ToString();
            }

            string portfolioId = "0";

            if(Request.RouteValues["controller"].ToString().ToLower() == "portfolios" && Request.RouteValues.ContainsKey("id"))
            {
                portfolioId = Request.RouteValues["id"].ToString();
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

            return result;
        }
    }
}
