using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FomoApi.Domain.Login;

namespace FomoApi.Infrastructure
{
    public partial class LoginContext: IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public LoginContext()
        {
        }

        public LoginContext(DbContextOptions<LoginContext> options)
            : base(options)
        {
        }    
    }
}
