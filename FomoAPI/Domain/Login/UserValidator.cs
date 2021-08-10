using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;

namespace FomoAPI.Domain.Login
{
    public class UserValidator : AbstractValidator<IdentityUser<Guid>>
    {
        public UserValidator(UserManager<IdentityUser<Guid>> userManager, IOptionsMonitor<UserValidationOptions> options)
        {         
            RuleFor(u => u.UserName).MaximumLength(options.CurrentValue.MaxLength);
            RuleFor(u => u.UserName).MinimumLength(options.CurrentValue.MinLength);
            RuleFor(u => u.UserName).MustAsync( async (name, cancel) => await userManager.FindByNameAsync(name) == null).WithMessage("User name already exists");
        }
    }
}
