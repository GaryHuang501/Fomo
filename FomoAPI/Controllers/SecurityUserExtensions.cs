using System;
using System.Security.Claims;

namespace FomoAPI.Controllers
{
    public static class SecurityUserExtensions
    {
        public const string UserNameClaimKey = "userName";
        
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name);
        }
    }
}
