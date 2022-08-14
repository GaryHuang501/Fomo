using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using FomoAPI.Infrastructure.Clients;
using FomoAPI.Application.DTOs;
using FomoAPI.Infrastructure.Repositories;
using System.Linq;
using FomoAPI.Controllers.Authorization;
using Microsoft.Extensions.Options;
using FomoAPI.Application.ConfigurationOptions;
using FomoAPI.Domain.Login;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace FomoAPI.Controllers
{
    /// <summary>
    /// Registering new users, logging in and fetching user  data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly SignInManager<IdentityUser<Guid>> _signInManager;
        private readonly UserManager<IdentityUser<Guid>> _userManager;
        private readonly IPortfolioRepository _portfolioRepo;
        private readonly IClientAuthFactory _clientAuthFactory;
        private readonly UserValidator _validator;
        private readonly ILogger<AccountsController> _logger;
        private readonly IOptionsMonitor<AccountsOptions> _accountsOptions;

        public AccountsController(SignInManager<IdentityUser<Guid>> signInManager, 
                                  UserManager<IdentityUser<Guid>> userManager,
                                  IPortfolioRepository portfolioRepo,
                                  IClientAuthFactory clientAuthFactory,
                                  UserValidator validator,
                                  ILogger<AccountsController> logger, 
                                  IOptionsMonitor<AccountsOptions> accountsOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _clientAuthFactory = clientAuthFactory;
            _validator = validator;
            _logger = logger;
            _portfolioRepo = portfolioRepo;
            _accountsOptions = accountsOptions;
        }

        /// <summary>
        /// Gets user info for current logged in user.
        /// </summary>
        /// <response code="200">Returns user account.</response>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<ActionResult<UserDTO>> GetAccount()
        {
            IdentityUser<Guid> myUser = await _userManager.GetUserAsync(User);

            var userDTO = new UserDTO(myUser.Id, myUser.UserName);

            return Ok(userDTO);
        }

        /// <summary>
        /// Gets user info for selected user.
        /// </summary>
        /// <response code="200">Returns user account.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<ActionResult<UserDTO>> GetAccount(string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                return BadRequest("UserId must be a valid Guid");
            }

            IdentityUser<Guid> selectedUser = await _userManager.FindByIdAsync(id);

            var userDTO = new UserDTO(selectedUser.Id, selectedUser.UserName);

            return Ok(userDTO);
        }

        /// <summary>
        /// Update User profile.
        /// </summary>
        /// <response code="200">Returns updated user account.</response>
        /// <response code="400">Update failed to validate.</response>
        /// <response code="403">Forbiddden from updating other user accounts.</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public async Task<ActionResult<UserDTO>> UpdateAccount([FromBody, Required] UserDTO userToUpdate)
        {
            if(userToUpdate.Id != User.GetUserId())
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Unauthorized to update this account.");
            }

            IdentityUser<Guid> myUser = await _userManager.FindByIdAsync(userToUpdate.Id.ToString());
            myUser.UserName = userToUpdate.Name;

            var validationResult = await _validator.ValidateAsync(myUser);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToString());
            }

            var result = await _userManager.SetUserNameAsync(myUser, userToUpdate.Name);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.First());
            }

            var updatedUser = await _userManager.FindByIdAsync(userToUpdate.Id.ToString());
            await _signInManager.RefreshSignInAsync(updatedUser);

            return Ok(new UserDTO(updatedUser.Id, updatedUser.UserName));
        }

        /// <summary>
        /// Gets client custom token to access third party API such as firebase.
        /// </summary>
        /// <response code="200">Returns client token as string.</response>
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpGet("ClientCustomToken")]
        [Produces("text/plain")]
        public async Task<ActionResult<string>> GetClientCustomToken()
        {
            var claims = new UserClaims(User);
            _logger.LogInformation("Token request for Login for {userName}", User.GetUserId());

            return await _clientAuthFactory.CreateClientToken(Guid.Parse(claims.UserID), claims.Value);
        }

        /// <summary>
        /// Log into the given provider
        /// </summary>
        /// <param name="provider">Provider to login to</param>
        /// <param name="returnUrl">Return URL after logging in</param>
        /// <response code="200">Returns challenge result of login.</response>
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("login")]
        public IActionResult Login([FromQuery, Required] string provider, [FromQuery, Required] string returnUrl)
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, $"{_accountsOptions.CurrentValue.ExternalCallBackUrl}?returnurl=" + returnUrl);
            return new ChallengeResult(provider, properties);
        }

        /// <summary>
        /// Login into demo account 
        /// </summary>
        /// <response code="200">Returns updated user account.</response>
        /// <response code="401">Invalid Demo credentials</response>
        [HttpGet("Login/Demo")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LoginDemo([FromQuery, Required] string returnUrl)
        {
            var demoUser = await _userManager.FindByIdAsync(_accountsOptions.CurrentValue.DemoUser.Id);
            var signInResult = await _signInManager.PasswordSignInAsync(demoUser.UserName, _accountsOptions.CurrentValue.DemoUser.Password, false, false);

            if (signInResult.Succeeded)
            {
                return Redirect(returnUrl);
            }

            return Unauthorized("Demo login failed");
        }

        /// <summary>
        /// Log out
        /// </summary>
        /// <response code="200">User is logged out.</response>
        [HttpGet("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExternalLoginCallback([FromQuery, Required] string returnUrl)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (loginInfo == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            var user = await _userManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

            if (user == null)
            {
                var newUser = new IdentityUser<Guid>
                {
                    UserName = _accountsOptions.CurrentValue.DefaultName,
                    Email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email)
                };

                var result = await RegisterUser(loginInfo, newUser);

                if (!result.Success)
                {
                    return StatusCode(result.StatusCode, result.Error);
                }

                return Redirect($"{_accountsOptions.CurrentValue.RegistrationPage}/{newUser.Id}");
            }
            else
            {
                if (!await SignInUser(loginInfo, user))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }


            return Redirect(returnUrl);      
        }

        private async Task<(bool Success, int StatusCode, string Error)> RegisterUser(ExternalLoginInfo loginInfo, IdentityUser<Guid> user)
        {
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return (false, 500, "Unable to register user");
            }

            var portfolio = await _portfolioRepo.CreatePortfolio(user.Id, _accountsOptions.CurrentValue.DefaultPortfolioName);

            await _userManager.AddClaimAsync(user, new Claim(FomoClaimTypes.PortfolioId, portfolio.Id.ToString()));

            result = await _userManager.AddLoginAsync(user, loginInfo);

            if (!result.Succeeded)
            {
                return (false, 500, "Failed to add external info to user");
            }

            await SignInUser(loginInfo, user);

            return (true, 200, null);
        }

        private async Task<bool> SignInUser(ExternalLoginInfo loginInfo, IdentityUser<Guid> user)
        {
            var result = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);

            if (!(await VerifyClaims(user)))
            {
                await _signInManager.RefreshSignInAsync(user);
            }

            if (!(await _clientAuthFactory.VerifyUser(user.Id)))
            {
                await _clientAuthFactory.CreateUser(user.Id, user.UserName, user.Email);
            }

            return result.Succeeded;
        }

        private async Task<bool> VerifyClaims(IdentityUser<Guid> user)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            bool isVerified = true;

            if (claims.FirstOrDefault(c => c.Type == FomoClaimTypes.PortfolioId) == null)
            {
                var portfolioIds = await _portfolioRepo.GetPortfolioIds(user.Id);
                await _userManager.AddClaimAsync(user, new Claim(FomoClaimTypes.PortfolioId, portfolioIds.First().ToString()));
                isVerified = false;
            }

            return isVerified;
        }
    }
}