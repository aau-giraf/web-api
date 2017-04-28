using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GirafRest.Models;
using GirafRest.Services;
using GirafRest.Models.DTOs.AccountDTOs;

namespace GirafRest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// A reference to ASP.NET's user manager, which handles currently authenticated users.
        /// </summary>
        private readonly UserManager<GirafUser> _userManager;
        /// <summary>
        /// A reference to ASP.NET's sign-in manager, that is used to validate usernames and passwords.
        /// </summary>
        private readonly SignInManager<GirafUser> _signInManager;
        /// <summary>
        /// A reference to an email sender, that is used to send emails to users who request a new password.
        /// </summary>
        private readonly IEmailSender _emailSender;
        /// <summary>
        /// A logger used to log information from the controller.
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// A cookie-scheme, this is automatically provided by ASP.NET.
        /// </summary>
        private readonly string _externalCookieScheme;

        /// <summary>
        /// Creates a new account controller. The account controller allows the users to sign in and out of their account
        /// as well as creating new users. The account controller is automatically instantiated by ASP.NET.
        /// </summary>
        /// <param name="userManager">A reference to a user manager.</param>
        /// <param name="signInManager">A reference to a sign in manager</param>
        /// <param name="emailSender">A reference to an implementation of the IEmailSender interface.</param>
        /// <param name="identityCookieOptions">A reference to a cookie-scheme.</param>
        /// <param name="loggerFactory">A reference to a logger factory</param>
        public AccountController(
            UserManager<GirafUser> userManager,
            SignInManager<GirafUser> signInManager,
            IEmailSender emailSender,
            IOptions<IdentityCookieOptions> identityCookieOptions,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _externalCookieScheme = identityCookieOptions.Value.ExternalCookieAuthenticationScheme;
            _logger = loggerFactory.CreateLogger<AccountController>();
        }

        /// <summary>
        /// This endpoint allows the user to sign in to his account by providing valid username and password.
        /// </summary>
        /// <param name="model">A LoginViewModel, i.e. a json-string with a username and a password field.</param>
        /// <returns>
        /// BadRequest if the caller fails to supply a valid username or password,
        /// Unauthorized if either the username or pass is not recognized
        /// or Ok if sign in was succesful.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            //Check that the caller has supplied username and password in the request
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("No username specified.");
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("No password specified.");

            //Attempt to sign in with the given credentials.
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(1, $"{model.Username} logged in.");
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        /// <summary>
        /// Register a new user in the REST-API. The caller must supply a username, a password and a ConfirmPassword.
        /// </summary>
        /// <param name="model">A refernece to a RegisterViewModel, i.e. a json string containing three strings;
        /// Username, Password and ConfirmPassword.</param>
        /// <returns>
        /// BadRequest if the request lacks some information or the user could not be created and
        /// Ok if the user was actually created.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            //Check that all the necesarry data has been supplied
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("Please supply a username.");
            if (string.IsNullOrEmpty(model.Password))
                return BadRequest("Please supply a password");
            if (string.IsNullOrEmpty(model.ConfirmPassword))
                return BadRequest("Please supply a ConfirmPassword");

            if (!model.Password.Equals(model.ConfirmPassword))
                return BadRequest("The Password and ConfirmPassword must be equal.");

            //Create a new user with the supplied information
            var user = new GirafUser (model.Username, model.DepartmentId);
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation(3, "User created a new account with password.");
                return Ok();
            }
            AddErrors(result);
            return BadRequest();
        }

        /// <summary>
        /// Log the currently authenticated user out of the system.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return Ok("You logged out.");
        }

        #region Password recovery DOES NOT WORK YET
        /// <summary>
        /// Use this endpoint to request a password reset link, which is send to the user's email address.
        /// </summary>
        /// <param name="model">A json string containing username and email.</param>
        /// <returns>
        /// BadRequest if the request does not contain all necesarry information,
        /// NotFound if the no user with the given username exists and 
        /// Ok if the user was found.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("No username was supplied");
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("No email was supplied");

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return NotFound("Please try again with another username");
            }
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
               $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            return Ok($"An email has been sent to {model.Email} with a password reset link.");
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Unauthorized();
        }
        #endregion

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        #endregion
    }
}
