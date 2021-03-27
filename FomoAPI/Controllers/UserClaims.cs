using FomoAPI.Controllers.Authorization;
using System.Collections.Generic;
using System.Security.Claims;

namespace FomoAPI.Controllers
{
    public class UserClaims
    {
        public string UserID { get; private set; }

        public IReadOnlyDictionary<string, object> Value { get; private set; }
        public UserClaims(ClaimsPrincipal principal)
        {
            UserID = principal.GetUserId().ToString();

            Value = new Dictionary<string, object>
            {
                { FomoClaimTypes.UserName, principal.GetUserName().ToString() },
            };
        }
    }
}
