using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using GirafRest.Models;
using GirafRest.Services;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Models.DTOs.UserDTOs;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Http;
using System;

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
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
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
            if (model == null)
                return BadRequest("The request body must contain username and password.");
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
            if (model == null)
                return BadRequest("Please specify both 'Username', 'Password' and 'ConfirmPassword' " +
                    "in the request body. You may optionally specify 'DepartmentId'.");
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
                return Ok(new GirafUserDTO(user));
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

        #region Password recovery
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
            if (model == null)
                return BadRequest("The request body must contain both a username and an email.");
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("No username was supplied");
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest("No email was supplied");

            string reply = $"An email has been sent to {model.Email} with a password reset link if the given username exists in the database.";

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok(reply);
            }
            
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Nulstil kodeord",
                   $"Du har mistet din kode til GIRAF. Du kan nulstille det her: <a href='{callbackUrl}'>Nulstil Kodeord</a>");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occured:\n{ex.Message}\nInner Exception:\n{ex.InnerException}");
                return BadRequest("The mailing service is currently offline. Please contact an administrator if the problem " +
                    "persists.");
            }
            return Ok(reply);
        }

        /// <summary>
        /// Creates a new password for the currently authenticated user.
        /// </summary>
        /// <param name="model">Information on the new password, i.e. a JSON string containing
        /// NewPassword and ConfirmPassword.</param>
        /// <returns>BadRequest if the server failed to update the password or Ok if everything went well.</returns>
        [HttpPost("set-password")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> SetPassword(SetPasswordDTO model)
        {
            if (model == null)
                return BadRequest("You must specify both 'NewPassword' and 'ConfirmPassword' in the request body.");
            if (string.IsNullOrEmpty(model.NewPassword))
                return BadRequest("Please add a 'NewPassword' field to the request.");
            if (string.IsNullOrEmpty(model.ConfirmPassword))
                return BadRequest("Please add a 'ConfirmPassword' field to the request.");
            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest("Password Mismatch.");

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok("Your password was set.");
                }
                AddErrors(result);
            }
            return BadRequest("Password update did not succeed." + ModelState["Identity"]);
        }
        /// <summary>
        /// Allows the user to change his password.
        /// </summary>
        /// <param name="model">All information needed to change the password, i.e. old password, new password
        /// and a confirmation of the new password.</param>
        /// <returns>BadRequest if something went wrong and ok if everything went well.</returns>
        [HttpPost("change-password")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO model)
        {
            if (model == null)
                return BadRequest("The request body must contain 'OldPassword', 'NewPassword' and 'ConfirmPassword'.");
            if (model.OldPassword == null || model.NewPassword == null || model.ConfirmPassword == null)
                return BadRequest("Please specify both you old password, a new one and a confirmation of the new one.");
            if (model.NewPassword != model.ConfirmPassword)
                return BadRequest("Password Mismatch");

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation("User changed their password successfully.");
                    return Ok("Your password was changed.");
                }
            }
            return BadRequest("An error occured: " + ModelState["Identity"]);
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
            var user = await _userManager.FindByNameAsync(model.Username);
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
                ModelState.AddModelError("Identity", error.Description);
            }
        }

        #endregion
    }
}
