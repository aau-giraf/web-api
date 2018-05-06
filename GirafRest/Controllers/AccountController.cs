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
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models.Responses;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("v1/[controller]")]
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
        /// Configuration
        /// </summary>
        private readonly IOptions<JwtConfig> _configuration;

        /// <summary>
        /// Creates a new account controller. The account controller allows the users to sign in and out of their account
        /// as well as creating new users. The account controller is automatically instantiated by ASP.NET.
        /// </summary>
        /// <param name="signInManager">A reference to a sign in manager</param>
        /// <param name="emailSender">A reference to an implementation of the IEmailSender interface.</param>
        /// <param name="loggerFactory">A reference to a logger factory</param>
        /// <param name="giraf">A reference to the implementation of the IGirafService interface.</param>
        /// <param name="configuration">A configuration object</param>
        /// <param name="roleManager">A roleManager object for finding user roles</param>
        public AccountController(
            SignInManager<GirafUser> signInManager,
            IEmailService emailSender,
            ILoggerFactory loggerFactory,
            IGirafService giraf,
            IOptions<JwtConfig> configuration,
            RoleManager<GirafRole> roleManager)
        {
            _signInManager = signInManager;
            _emailSender = emailSender;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
            _configuration = configuration;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Generates a JSON Web Token Token (JwtToken) for a given user and role. Based on the method with the same name from https://github.com/jatarga/WebApiJwt/blob/master/Controllers/AccountController.cs
        /// </summary>
        /// <param name="user">Which user</param>
        /// <param name="roles">Which roles</param>
        /// <returns>
        /// The Token as a string
        /// </returns>
        private async Task<string> GenerateJwtToken(GirafUser user, string impersonatedBy, GirafRoles roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("impersonatedBy", impersonatedBy ?? ""),
            };

            claims.AddRange(await GetRoleClaims(user));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.JwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration.Value.JwtExpireDays));

            var token = new JwtSecurityToken(
                _configuration.Value.JwtIssuer,
                _configuration.Value.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Gets roles s.t we can get role from payload 
        /// </summary>
        /// <returns>The role claims.</returns>
        /// <param name="user">User.</param>
        private async Task<List<Claim>> GetRoleClaims(GirafUser user){
            var roleclaims = new List<Claim>();
            var userRoles = await _giraf._userManager.GetRolesAsync(user);
            roleclaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            return roleclaims;
        }

        /// <summary>
        /// This endpoint allows the user to sign in to his account by providing valid username and password.
        /// </summary>
        /// <param name="model">A LoginDTO(LoginViewModelDTO), i.e. a json-string with a username and a password field.</param>
        /// <returns>
        /// JwtToken if credentials are valid
        /// MissingProperties if any information in body is null or empty
        /// InvalidCredentials if invalid username or password
        /// </returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<Response<string>> Login([FromBody]LoginDTO model)
        {
            if (model == null)
                return new ErrorResponse<string>(ErrorCode.MissingProperties, "model");
            //Check that the caller has supplied username in the request
            if (string.IsNullOrEmpty(model.Username))
                return new ErrorResponse<string>(ErrorCode.MissingProperties, "username");

            //Check if a user is already logged in and attempt to login with the username given in the DTO
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            var loginUser =  _giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username);
            if (loginUser == null) // If username is invalid
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials, "username");
            var userRoles = await _roleManager.findUserRole(_giraf._userManager, loginUser);

            if (string.IsNullOrEmpty(model.Password))
            {
                return new ErrorResponse<string>(ErrorCode.MissingProperties, "password");
            }
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials);

            return new Response<string>(await GenerateJwtToken(loginUser, loginUser.Id, userRoles));

        }

        /// <summary>
        /// Attempts to login from to a user's account from one of his superior's. This allows departments
        /// to login as Guardians and guardians to login as citizens. The superiors does not require 
        /// password in order to login, but they must be in the same department. 
        /// </summary>
        /// <param name="superior">The Guardian user who is currently authenticated.</param>
        /// <param name="username">The username of the citizen to login as.</param>
        /// <param name="role">A string describing which role the target user is in.</param>
        /// <returns>
        /// A response containing a JwtToken or an ErrorReponse
        /// </returns>
        private async Task<Response<string>> AttemptRoleLoginTokenAsync(GirafUser superior, string username, string role)
        {
            //Attempt to find a user with the given username in the guardian's department
            var loginUser = _giraf._context.Users.FirstOrDefault(u => u.UserName == username);

            if (loginUser != null && loginUser.DepartmentKey == superior.DepartmentKey)
            {
                if (!await _giraf._userManager.IsInRoleAsync(loginUser, role))
                    return new ErrorResponse<string>(ErrorCode.NotAuthorized);

                await _signInManager.SignOutAsync();
                await _signInManager.SignInAsync(loginUser, isPersistent: true);

                // Get the roles the user is associated with
                GirafRoles userRoles = await _roleManager.findUserRole(_giraf._userManager, loginUser);

                return new Response<string>(await GenerateJwtToken(loginUser, User.Claims.FirstOrDefault(c => c.Type == "impersonatedBy")?.Value, userRoles));
            }

            //There was no user with the given username in the department - return invalidcredentials.
            else
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials);
        }

        /// <summary>
        /// Register a new user in the REST-API
        /// </summary>
        /// <param name="model">A reference to a RegisterDTO(RegisterViewModelDTO), i.e. a json string containing three strings;
        /// Username and Password.</param>
        /// <returns>
        /// Response with a GirafUserDTO with either the new user or an error
        /// </returns>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<Response<GirafUserDTO>> Register([FromBody] RegisterDTO model)
        {
            if(model == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);
            //Check that all the necesarry data has been supplied
            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);

            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.InvalidCredentials);
            var doesUserAlreadyExist = (_giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username) != null);

            if (doesUserAlreadyExist)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyExists);

            Department department = await _giraf._context.Departments.Where(dep => dep.Key == model.DepartmentId).FirstOrDefaultAsync();

            // Check that the department with the specified id exists
            if (department == null && model.DepartmentId != null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.DepartmentNotFound);

            //Create a new user with the supplied information
            var user = new GirafUser (model.Username, department);
            var result = await _giraf._userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _giraf._userManager.AddToRoleAsync(user, GirafRole.Citizen);
                await _signInManager.SignInAsync(user, isPersistent: true);
                _giraf._logger.LogInformation("User created a new account with password.");

                // if department != null
                // Get the roles the user is associated with
                if (department != null)
                {
                    // Add a relation to all the newly created citizens guardians
                    var roleGuardianId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Guardian)
                                               .Select(c => c.Id).FirstOrDefault();

                    var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleGuardianId)
                                        .Select(r => r.UserId).Distinct();

                    var guardians = _giraf._context.Users.Where(u => userIds.Any(ui => ui == u.Id)
                                                                && u.DepartmentKey == department.Key).ToList();

                    foreach (var guardian in guardians)
                    {
                        user.AddGuardian(guardian);
                    }
                    await _giraf._context.SaveChangesAsync();
                }
                // fetch the roleenum
                GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);
                // return the created user
                return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
            }
            AddErrors(result);
            return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);
        }

        /// <summary>
        /// Logs the currently authenticated user out of the system.
        /// </summary>
        /// <returns>
        /// A response object
        /// </returns>
        [HttpPost("logout")]        
        [AllowAnonymous]
        public async Task<Response> Logout()
        {
            await _signInManager.SignOutAsync();
            _giraf._logger.LogInformation("User logged out.");
            return new Response();
        }

        #region Password recovery
        /// <summary>
        /// Use this endpoint to request a password reset link, which is send to the user's email address.
        /// </summary>
        /// <param name="model">A ForgotPasswordDTO, which contains a username and an email address.</param>
        /// <returns>
        /// An empty response if succesfull or an ErrorResponse if not succesfull
        /// </returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<Response> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            if (model == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "model");
            if (string.IsNullOrEmpty(model.Username))
                return new ErrorResponse(ErrorCode.MissingProperties, "username");
            if (string.IsNullOrEmpty(model.Email))
                return new ErrorResponse(ErrorCode.MissingProperties, "email");

            string reply = $"An email has been sent to {model.Email} with a password reset link if the given username exists in the database.";

            var user = await _giraf._userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return new Response();
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
                return new ErrorResponse(ErrorCode.EmailServiceUnavailable);
            }
            return new Response();
        }

        /// <summary>
        /// Creates a new password for the currently authenticated user.
        /// </summary>
        /// <param name="model">Information on the new password in a SetPasswordDTO, i.e. a JSON string containing and NewPassword.</param>
        /// <returns>
        /// Empty Response on success. 
        /// MissingProperties if there was missing properties
        /// PasswordNotUpdated if the user wasn't logged in
        /// </returns>
        [HttpPost("set-password")]
        public async Task<Response> SetPassword(SetPasswordDTO model)
        {
            if (model == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword");
            if (string.IsNullOrEmpty(model.NewPassword))
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword");

            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return new Response();
                }
                AddErrors(result);
            }
            return new ErrorResponse(ErrorCode.PasswordNotUpdated);
        }
        /// <summary>
        /// Allows the user to change his password.
        /// </summary>
        /// <param name="model">All information needed to change the password in a ChangePasswordDTO, i.e. old password, new password
        /// and a confirmation of the new password.</param>
        /// <returns>
        /// Empty Response on success. 
        /// MissingProperties if there was missing properties
        /// PasswordNotUpdated if the user wasn't logged in
        /// </returns>
        [HttpPost("change-password")]
        public async Task<Response> ChangePassword(ChangePasswordDTO model)
        {
            if (model == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword", "oldPassword");
            if (model.OldPassword == null || model.NewPassword == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword", "oldPassword");

            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                var result = await _giraf._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _giraf._logger.LogInformation("User changed their password successfully.");
                    return new Response();
                }
            }
            return new ErrorResponse(ErrorCode.PasswordNotUpdated);
        }

        /// <summary>
        /// Gets the view associated with the ResetPassword page.
        /// </summary>
        /// <param name="code">The reset password token that has been sent to the user via his email.</param>
        /// <returns>
        /// BadRequest if there is no valid code or the view if the code was valid.
        /// </returns>
        [HttpGet("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
                return RedirectToAction("Error");
            return View();
        }

        /// <summary>
        /// Attempts to change the given user's password. If the DTO did not contain valid information simply returns the view with
        /// the current information that the user has specified.
        /// </summary>
        /// <param name="model">A DTO containing the user's Username and Password.</param>
        /// <returns>
        /// The view if password was wrong or redirects to ResetPasswordConfirmation.
        /// </returns>
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
        /// <returns>
        /// The view.
        /// </returns>
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
        /// <returns>
        /// Unauthorized.
        /// </returns>
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
