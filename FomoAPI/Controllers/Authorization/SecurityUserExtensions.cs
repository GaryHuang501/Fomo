using System;
using System.Security.Claims;

namespace FomoAPI.Controllers.Authorization
{
    public static class SecurityUserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static int GetPortfolioId(this ClaimsPrincipal user)
        {
            return int.Parse(user.FindFirstValue(FomoClaimTypes.PortfolioId));
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }
    }
}
