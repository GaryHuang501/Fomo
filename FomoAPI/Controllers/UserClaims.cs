using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
                { SecurityUserExtensions.UserNameClaimKey, principal.GetUserName().ToString() },
            };
        }
    }
}
