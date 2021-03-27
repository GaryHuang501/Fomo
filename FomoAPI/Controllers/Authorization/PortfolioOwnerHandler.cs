using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace FomoAPI.Controllers.Authorization
{
    public class PortfolioOwnerHandler : AuthorizationHandler<PortfolioOwnerRequirement>
    {
        IHttpContextAccessor _httpContextAccessor;

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

            if (!_httpContextAccessor.HttpContext.Request.RouteValues.ContainsKey("id"))
            {
                return Task.CompletedTask;
            }

            var portfolioId = _httpContextAccessor.HttpContext.Request.RouteValues["id"].ToString();

            if (portfolioId == context.User.GetPortfolioId().ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
