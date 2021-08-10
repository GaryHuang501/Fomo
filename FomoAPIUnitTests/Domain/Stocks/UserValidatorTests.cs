using FomoAPI.Domain.Login;
using FomoAPI.Domain.Stocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FomoAPIUnitTests.Domain.Stocks
{
    public class UserValidatorTests
    {
        [Theory]
        [InlineData("Test", true)]
        [InlineData("I am a name", true)]
        [InlineData("I am a very very very very very long name", false)]
        [InlineData("", false)]
        [InlineData("12", false)]
        public async Task Should_ValidateNameLength(string name, bool expected)
        {
            var mockOptions = new Mock<IOptionsMonitor<UserValidationOptions>>();
            var mockUserManager = new Mock<UserManager<IdentityUser<Guid>>>(new Mock<IUserStore<IdentityUser<Guid>>>().Object, null, null, null, null, null, null, null, null);

            mockOptions.Setup(o => o.CurrentValue).Returns(new UserValidationOptions
            {
                MinLength = 3,
                MaxLength = 20
            });

            mockUserManager.Setup(n => n.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<IdentityUser<Guid>>(null));

            var validator = new UserValidator(mockUserManager.Object, mockOptions.Object);

            var user = new IdentityUser<Guid> { UserName = name };

            var result = await validator.ValidateAsync(user);

            Assert.Equal(expected, result.IsValid);
        }

        [Theory]
        [InlineData("Not duplicate", true)]
        [InlineData("Duplicate", false)]
        public async Task Should_ValidateForDuplicateNames(string name, bool expected)
        {
            var mockOptions = new Mock<IOptionsMonitor<UserValidationOptions>>();
            var mockUserManager = new Mock<UserManager<IdentityUser<Guid>>>(new Mock<IUserStore<IdentityUser<Guid>>>().Object, null, null, null, null, null, null, null, null);

            mockOptions.Setup(o => o.CurrentValue).Returns(new UserValidationOptions
            {
                MinLength = 0,
                MaxLength = 9999
            });

            IdentityUser<Guid> foundUser = null;

            if (!expected)
            {
                foundUser = new IdentityUser<Guid> { UserName = name };
            }

            mockUserManager.Setup(n => n.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult<IdentityUser<Guid>>(foundUser));

            var validator = new UserValidator(mockUserManager.Object, mockOptions.Object);

            var user = new IdentityUser<Guid> { UserName = name };

            var result = await validator.ValidateAsync(user);

            Assert.Equal(expected, result.IsValid);
        }
    }
}
