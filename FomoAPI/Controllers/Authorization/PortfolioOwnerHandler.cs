using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FomoAPI.Controllers.Authorization
{
    public class PortfolioOwnerHandler : AuthorizationHandler<PortfolioOwnerRequirement>
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public PortfolioOwnerHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PortfolioOwnerRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == FomoClaimTypes.PortfolioId))
            {
                return Task.CompletedTask;
            }


            if(!TryGetPortfolioIdFromPath(out string portfolioId))
            {
                return Task.CompletedTask;
            }

            if (portfolioId == context.User.GetPortfolioId().ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool TryGetPortfolioIdFromPath(out string portfolioId)
        {
            portfolioId = null;

            var url = _httpContextAccessor.HttpContext.Request.Path.Value;

            var parts = url.Split("/");
            var portfolioIndex = Array.FindIndex(parts, p => p.Equals("portfolios", System.StringComparison.OrdinalIgnoreCase)) ;
            var portfolioIndexId = portfolioIndex + 1;
            bool portfolioIdExists = portfolioIndexId <= parts.Length;

            if (portfolioIndex < 0 || !portfolioIdExists)
            {
                return false;
            }

            portfolioId = parts[portfolioIndexId];

            return true;
        }
    }
}
