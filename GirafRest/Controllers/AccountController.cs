using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GirafRest.Models;
using GirafRest.Services;
using GirafRest.Extensions;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Models.DTOs.UserDTOs;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Http;
using System;
using static GirafRest.Models.DTOs.GirafUserDTO;
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
        /// <summary>
        /// Reference to the GirafService, which contains helper methods used by most controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        /// <summary>
        /// A reference to the role manager for the project.
        /// </summary>
        private readonly RoleManager<GirafRole> _roleManager;
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
            IGirafService giraf,
            RoleManager<GirafRole> roleManager)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
            _roleManager = roleManager;
        }

        /// <summary>
        /// This endpoint allows the user to sign in to his account by providing valid username and password.
        /// </summary>
        /// <param name="model">A LoginDTO(LoginViewModelDTO), i.e. a json-string with a username and a password field.</param>
        /// <returns>
        /// BadRequest if the caller fails to supply a valid username or password,
        /// Unauthorized if either the username or password is not recognized or if a Guardian/Department attempted to log on
        /// to another user that is not in their department.
        /// Ok if sign in was succesful.
        /// </returns>
        [HttpPost("login")]
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
                    return await attemptRoleLoginAsync(currentUser, model.Username, GirafRole.Citizen);
                }
                else if(await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department)){
                    _giraf._logger.LogInformation("Department attempted to sign in as Guardian");
                    return await attemptRoleLoginAsync(currentUser, model.Username, GirafRole.Guardian);
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
        /// Attempts to login from to a user's account from one of his supperior's. This allows departments
        /// to login as Guardians and guardians to login as citizens. The superiors does not require 
        /// password in order to login, but they must be in the same department. 
        /// </summary>
        /// <param name="superior">The Guardian user who is currently authenticated.</param>
        /// <param name="username">The username of the citizen to login as.</param>
        /// <param name="role">A string describing which role the target user is in.</param>
        /// <returns></returns>
        private async Task<IActionResult> attemptRoleLoginAsync(GirafUser superior, string username, string role)
        {
            //Attempt to find a user with the given username in the guardian's department
            var loginUser = await _giraf.LoadByNameAsync(username);
            
            if (loginUser != null && loginUser.DepartmentKey == superior.DepartmentKey)
            {
                if (!await _giraf._userManager.IsInRoleAsync(loginUser, role))
                    return Unauthorized();

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(loginUser, isPersistent: true);

                // Get the roles the user is associated with
                GirafRoles userRoles = await _roleManager.findUserRole(_giraf._userManager, loginUser);

                return Ok(new GirafUserDTO(loginUser, userRoles));
            }
            
            //There was no user with the given username in the department - return NotFound.
            else
                return NotFound($"There is no user with the given username in your department: {username}");
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
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            //Check that all the necesarry data has been supplied
            if (!ModelState.IsValid)
                return BadRequest("Some data was missing from the serialized object \n\n" +
                                  string.Join(",",
                                  ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray()));

            // Check that password and confirm password match
            if (!model.Password.Equals(model.ConfirmPassword))
                return BadRequest("The Password and ConfirmPassword must be equal.");

            var department = await _giraf._context.Departments.Where(dep => dep.Key == model.DepartmentId).FirstOrDefaultAsync();

            // Check that the department with the specified id exists
            if (department == null)
                return BadRequest("Department does not exist");

            //Create a new user with the supplied information
            var user = new GirafUser (model.Username, department);
            var result = await _giraf._userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _giraf._userManager.AddToRoleAsync(user, GirafRole.Citizen);
                await _signInManager.SignInAsync(user, isPersistent: true);
                _giraf._logger.LogInformation("User created a new account with password.");

                // Get the roles the user is associated with
                GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

                return Ok(new GirafUserDTO(user, userRole));
            }
            AddErrors(result);
            return BadRequest();
        }

        /// <summary>
        /// Logs the currently authenticated user out of the system.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpPost("logout")]
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
        [HttpPost("forgot-password")]
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
        /// <param name="model">All information needed to change the password in a ChangePasswordDTO, i.e. old password, new password
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
        [HttpGet("reset-password")]
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
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO model)
        {
            //Check that all the necessary information was specified
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
        [HttpGet("reset-password-confirmation")]
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
        [HttpGet("access-denied")]
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
