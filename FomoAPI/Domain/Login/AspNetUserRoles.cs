using System;
using System.Collections.Generic;

namespace FomoApi.Domain.Login
{
    public partial class AspNetUserRoles
    {
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
    }
}
