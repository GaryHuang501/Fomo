using System;
using System.Security.Claims;

namespace FomoAPI.Controllers
{
    public static class SecurityUserExtensions
    {

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
