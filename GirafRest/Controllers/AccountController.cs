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
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// A reference to ASP.NET's sign-in manager, that is used to validate usernames and passwords.
        /// </summary>
        private readonly SignInManager<GirafUser> _signInManager;
        /// <summary>
        /// A reference to an email sender, that is used to send emails to users who request a new password.
        /// </summary>
        private readonly IEmailService _emailSender;
        private readonly IGirafService _giraf;

        /// <summary>
        /// Creates a new account controller. The account controller allows the users to sign in and out of their account
        /// as well as creating new users. The account controller is automatically instantiated by ASP.NET.
        /// </summary>
        /// <param name="signInManager">A reference to a sign in manager</param>
        /// <param name="emailSender">A reference to an implementation of the IEmailSender interface.</param>
        /// <param name="giraf">A reference to the implementation of the IGirafService interface.</param>
        /// <param name="loggerFactory">A reference to a logger factory</param>
        public AccountController(
            SignInManager<GirafUser> signInManager,
            IEmailService emailSender,
            ILoggerFactory loggerFactory,
            IGirafService giraf)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
        }

        /// <summary>
        /// This endpoint allows the user to sign in to his account by providing valid username and password.
        /// </summary>
        /// <param name="model">A LoginDTO(LoginViewModelDTO), i.e. a json-string with a username and a password field.</param>
        /// <returns>
        /// BadRequest if the caller fails to supply a valid username or password,
        /// Unauthorized if either the username or pass is not recognized or if a Guardian/Department attempted to log on
        /// to another user that is not in their department.
        /// Ok if sign in was succesful.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (model == null)
                return BadRequest("The request body must contain at least a username.");
            //Check that the caller has supplied username and password in the request
            if (string.IsNullOrEmpty(model.Username))
                return BadRequest("No username specified.");

            //Check if a user is already logged in and attempt to login with the username given in the DTO
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(currentUser != null)
            {
                if(await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian)){
                    _giraf._logger.LogInformation("Guardian attempted to sign in as Citizen");
                    return await attemptCitizenLoginAsync(currentUser, model.Username);
                }
                else{
                    _giraf._logger.LogInformation("Department attempted to sign in as Guardian");
                    return await attemptGuardianLoginAsync(currentUser, model.Username);
                }
            }

            //There is no current user - check that a password is present.
            if(string.IsNullOrEmpty(model.Password))
                return BadRequest("No password specified.");
            //Attempt to sign in with the given credentials.
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _giraf._logger.LogInformation($"{model.Username} logged in.");
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        /// <summary>
        /// Attempts to login from a Guardian's account to a citizen's account. Guardians does not require the citizen's 
        /// password in order to login, but they must be in the same department. 
        /// </summary>
        /// <param name="guardian">The Guardian user who is currently authenticated.</param>
        /// <param name="username">The username of the citizen to login as.</param>
        /// <returns></returns>
        private async Task<IActionResult> attemptCitizenLoginAsync(GirafUser guardian, string username)
        {
            //Check if the user is in the Guardian role - return unauthorized if not.
            if (await _giraf._userManager.IsInRoleAsync(guardian, GirafRole.Guardian))
            {
                //Attempt to find a user with the given username in the guardian's department
                var citizenUser = await _giraf.LoadByNameAsync(username);
                
                //Check if the user exists, sign out the guardian and sign in the user if so
                if (citizenUser != null && citizenUser.DepartmentKey == guardian.DepartmentKey)
                {
                    if (!await _giraf._userManager.IsInRoleAsync(citizenUser, GirafRole.Citizen))
                        return Unauthorized();

                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(citizenUser, isPersistent: true);
                    return Ok(new GirafUserDTO(citizenUser));
                }
                //There was no user with the given username in the department - return NotFound.
                else
                    return NotFound($"There is no user with the given username in your department: {username}");
            }
            else
                return Unauthorized();
        }
        
        /// <summary>
        /// Attempts to login from a Department account to a guardian account. Departments does not require the guardians 
        /// password in order to login, but they the guardian must be in the department. 
        /// </summary>
        /// <param name="department">The Department user who is currently authenticated.</param>
        /// <param name="username">The username of the guardian to login as.</param>
        /// <returns></returns>

        private async Task<IActionResult> attemptGuardianLoginAsync(GirafUser department, string username)
        {
            //Check if the user is in the Guardian role - return unauthorized if not.
            if (await _giraf._userManager.IsInRoleAsync(department, GirafRole.Department))
            {
                //Attempt to find a user with the given username in the guardian's department
                var guardianUser = await _giraf.LoadByNameAsync(username);
                
                //Check if the user exists, sign out the guardian and sign in the user if so
                if (guardianUser != null && guardianUser.DepartmentKey == department.DepartmentKey)
                {
                    if (!await _giraf._userManager.IsInRoleAsync(guardianUser, GirafRole.Guardian))
                        return Unauthorized();

                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(guardianUser, isPersistent: true);
                    return Ok(new GirafUserDTO(guardianUser));
                }
                //There was no user with the given username in the department - return NotFound.
                else
                    return NotFound($"There is no user with the given username in your department: {username}");
            }
            else
                return Unauthorized();
        }

        /// <summary>
        /// Register a new user in the REST-API. The caller must supply a username, a password and a ConfirmPassword.
        /// </summary>
        /// <param name="model">A reference to a RegisterDTO(RegisterViewModelDTO), i.e. a json string containing three strings;
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
            var result = await _giraf._userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _giraf._userManager.AddToRoleAsync(user, GirafRole.Citizen);
                await _signInManager.SignInAsync(user, isPersistent: true);
                _giraf._logger.LogInformation("User created a new account with password.");
                return Ok(new GirafUserDTO(user));
            }
            AddErrors(result);
            return BadRequest();
        }

        /// <summary>
        /// Logs the currently authenticated user out of the system.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _giraf._logger.LogInformation("User logged out.");
            return Ok("You logged out.");
        }

        #region Password recovery
        /// <summary>
        /// Use this endpoint to request a password reset link, which is send to the user's email address.
        /// </summary>
        /// <param name="model">A ForgotPasswordDTO, which contains a username and an email address.</param>
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

            var user = await _giraf._userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return Ok(reply);
            }
            
            var code = await _giraf._userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
            try
            {
                await _emailSender.SendEmailAsync(model.Email, "Nulstil kodeord",
                   $"Du har mistet din kode til GIRAF. Du kan nulstille det her: <a href='{callbackUrl}'>Nulstil Kodeord</a>");
            }
            catch (Exception ex)
            {
                _giraf._logger.LogError($"An exception occured:\n{ex.Message}\nInner Exception:\n{ex.InnerException}");
                return BadRequest("The mailing service is currently offline. Please contact an administrator if the problem " +
                    "persists.");
            }
            return Ok(reply);
        }

        /// <summary>
        /// Creates a new password for the currently authenticated user.
        /// </summary>
        /// <param name="model">Information on the new password in a SetPasswordDTO, i.e. a JSON string containing
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

            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.AddPasswordAsync(user, model.NewPassword);
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

            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _giraf._logger.LogInformation("User changed their password successfully.");
                    return Ok("Your password was changed.");
                }
            }
            return BadRequest("An error occured: " + ModelState["Identity"]);
        }

        /// <summary>
        /// Gets the view associated with the ResetPassword page.
        /// </summary>
        /// <param name="code"The reset password token that has been sent to the user via his email.></param>
        /// <returns>BadRequest if there is no valid code or the view if the code was valid.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
                return BadRequest("Failed to find a valid reset code.");
            return View();
        }

        /// <summary>
        /// Attempts to change the given user's password. If the DTO did not contain valid information simply returns the view with
        /// the current information that the user has specified.
        /// </summary>
        /// <param name="model">A DTO containing the user's Username, Password and a ConfirmPassword.</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            //Check that all the necesarry information was specified
            if (!ModelState.IsValid)
            {
                //It was not, return the user to the view, where he may try again.
                return View(model);
            }
            //Try to fetch the user from the given username. If the username does not exist simply return the
            //confirmation page to avoid revealing that the username does not exist.
            var user = await _giraf._userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            //Attempt to change the user's password and redirect him to the confirmation page.
            var result = await _giraf._userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        /// <summary>
        /// Get the view associated with the ResetPasswordConfirmation page.
        /// </summary>
        /// <returns>The view.</returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        /// <summary>
        /// An end-point that simply returns Unauthorized. It is redirected to by the runtime when an unauthorized request
        /// to an end-point with the [Authorize] attribute is encountered.
        /// </summary>
        /// <returns>Unauthorized.</returns>
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return Unauthorized();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Adds all the Identity errors to the model state, such that they may be displayed on the webpage.
        /// </summary>
        /// <param name="result">The IdentityResult containing all errors that were encountered.</param>
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
