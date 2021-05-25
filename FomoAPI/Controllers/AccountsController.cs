using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FomoApp.Exceptions;
using System;
using FomoAPI.Infrastructure.Clients;
using FomoAPI.Application.DTOs;
using FomoAPI.Infrastructure.Repositories;
using System.Linq;
using FomoAPI.Controllers.Authorization;

namespace FomoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IClientAuthFactory _clientAuthFactory;

        public AccountsController(SignInManager<IdentityUser<Guid>> signInManager, 
                                  UserManager<IdentityUser<Guid>> userManager,
                                  IPortfolioRepository portfolioRepo,
                                  IClientAuthFactory clientAuthFactory)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _clientAuthFactory = clientAuthFactory;
            _portfolioRepo = portfolioRepo;
        }

        /// <summary>
        /// Gets user info for current logged in user.
        /// </summary>
        /// <returns>Returns <see cref="UserDTO"/>if authorized</returns>
        [HttpGet("")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetAccount()
        {
            IdentityUser<Guid> myUser = await _userManager.GetUserAsync(User);

            var userDTO = new UserDTO(myUser.Id, myUser.UserName);

            return Ok(userDTO);
        }

        /// <summary>
        /// Gets user info for selected user.
        /// </summary>
        /// <returns>Returns <see cref="LoginInfoDTO"/>if authorized</returns>
        [HttpGet("{userId}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> GetAccount(string userId)
        {
            if (!Guid.TryParse(userId, out Guid selectedUserGuid))
            {
                return BadRequest("UserId must be a valid Guid");
            }

            IdentityUser<Guid> selectedUser = await _userManager.FindByIdAsync(userId);

            var userDTO = new UserDTO(selectedUser.Id, selectedUser.UserName);

            return Ok(userDTO);
        }

        /// <summary>
        /// Gets client custom token to access third party API such as firebase.
        /// </summary>
        /// <returns>Client Token.</returns>
        [HttpGet("ClientCustomToken")]
        [Authorize]
        public async Task<ActionResult<string>> GetClientCustomToken()
        {
            var claims = new UserClaims(User);

            return await _clientAuthFactory.CreateClientToken(Guid.Parse(claims.UserID), claims.Value);
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
        /// Log out
        /// </summary>
        /// <returns></returns>
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();

            return Redirect(returnUrl);
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
                bool success = false;

                if (user == null)
                {
                    user = await RegisterUser(loginInfo);
                    await _signInManager.SignInAsync(user, false);
                    success = true;
                }
                else
                {
                    var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                    success = result.Succeeded;
                }

                if (!(await VerifyClaims(user)))
                {
                    await _signInManager.RefreshSignInAsync(user);
                }

                if (!(await _clientAuthFactory.VerifyUser(user.Id)))
                {
                    await _clientAuthFactory.CreateUser(user.Id, user.UserName, user.Email);
                }

                if (success)
                {
                    return Redirect(returnUrl);
                }
            }

            return StatusCode(StatusCodes.Status500InternalServerError);

        }

        private async Task<bool> VerifyClaims(IdentityUser<Guid> user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            bool isVerified = true;

            if(claims.FirstOrDefault(c => c.Type == FomoClaimTypes.PortfolioId) == null)
            {
                var portfolioIds = await _portfolioRepo.GetPortfolioIds(user.Id);
                await _userManager.AddClaimAsync(user, new Claim(FomoClaimTypes.PortfolioId, portfolioIds.First().ToString()));
                isVerified = false;
            }

            return isVerified;
        }

        private async Task<IdentityUser<Guid>> RegisterUser(ExternalLoginInfo loginInfo)
        {
            var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            var emailUserNamePart = email.Split('@')[0];

            if (string.IsNullOrWhiteSpace(emailUserNamePart))
            {
                throw new UserRegistrationException("Empty user name received from email");
            }

            var user = new IdentityUser<Guid> { UserName = emailUserNamePart, Email = email };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException("Unable to register user");
            }

            var portfolio = await _portfolioRepo.CreatePortfolio(user.Id, "default");

            await _userManager.AddClaimAsync(user, new Claim(FomoClaimTypes.PortfolioId, portfolio.Id.ToString()));

            result = await _userManager.AddLoginAsync(user, loginInfo);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException("Failed to add external info to user");
            }

            return user;
        }
    }
}