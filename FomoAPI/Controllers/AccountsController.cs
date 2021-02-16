using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FomoApp.Exceptions;
using System;
using FomoAPI.Infrastructure.Clients;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IClientAuthFactory _clientAuthFactory;

        public AccountsController(SignInManager<IdentityUser<Guid>> signInManager, 
                                  UserManager<IdentityUser<Guid>> userManager,
                                  IClientAuthFactory clientAuthFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _clientAuthFactory = clientAuthFactory;
        }

        /// <summary>
        /// Check if the user's cookies are valid.
        /// </summary>
        /// <returns>Returns OK if authorized</returns>
        [HttpGet("CheckLogin")]
        [Authorize]
        public IActionResult CheckLogin()
        {
            return Ok();
        }

        /// <summary>
        /// Gets client custom token to access third party API such as firebase.
        /// </summary>
        /// <returns>Client Token.</returns>
        [HttpGet("ClientCustomToken")]
        [Authorize]
        public async Task<ActionResult<string>> GetClientCustomToken()
        {    
            return await _clientAuthFactory.CreateClientToken(User.GetUserId().ToString());
        }

        /// <summary>
        /// Log into the given provider
        /// </summary>
        /// <param name="provider">Provider to login to</param>
        /// <param name="returnUrl">Return URL after logging in</param>
        /// <returns></returns>
        [HttpGet("Login")]
        [AllowAnonymous]
        public IActionResult Login(string provider, string returnUrl)
        {

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, "/api/accounts/ExternalLoginCallback?returnurl=" + returnUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Callback after external provider to login or register new user
        /// </summary>
        /// <param name="returnUrl">Return URL after logging in</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo != null)
            {

                var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

                if (user == null)
                {
                    user = await RegisterUser(loginInfo);
                    await _signInManager.SignInAsync(user, false);
                    return Redirect(returnUrl);
                }
                else
                {
                    var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);


                    if (result.Succeeded)
                    {
                        return Redirect(returnUrl);
                    }
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);

        }

        private async Task<IdentityUser<Guid>> RegisterUser(ExternalLoginInfo loginInfo)
        {
            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
            var user = new IdentityUser<Guid> { UserName = email, Email = email };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException("Unable to register user");
            }

            result = await _userManager.AddLoginAsync(user, loginInfo);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException("Failed to add external info to user");
            }

            return user;
        }

    }
}