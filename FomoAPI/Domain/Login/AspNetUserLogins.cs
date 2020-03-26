using System;
using System.Collections.Generic;

namespace FomoApi.Domain.Login
{
    public partial class AspNetUserLogins
    {
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public Guid UserId { get; set; }

        public AspNetUsers User { get; set; }
    }
}
