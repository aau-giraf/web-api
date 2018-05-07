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
            var loginUser = await _giraf.LoadByNameAsync(model.Username);
            if (loginUser == null) // If username is invalid
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials, "username");
            var userRoles = await _roleManager.findUserRole(_giraf._userManager, loginUser);

            Microsoft.AspNetCore.Identity.SignInResult result = null;

            // If no current authenticated user it is safe to see if we can login
            if (currentUser == null)
            {
                if (string.IsNullOrEmpty(model.Password))
                {
                    return new ErrorResponse<string>(ErrorCode.MissingProperties, "password");
                }
                result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);
                if(result.Succeeded) return new Response<string>(await GenerateJwtToken(loginUser, loginUser.Id, userRoles));
                else return new ErrorResponse<string>(ErrorCode.InvalidCredentials);
            }

            /* If user is already logged in */
            else
            { 
                // If password is provided we change the current user
                if (!string.IsNullOrEmpty(model.Password))
                {
                    result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);
                    if (!result.Succeeded) return new ErrorResponse<string>(ErrorCode.InvalidCredentials);
                    return new Response<string>(await GenerateJwtToken(loginUser, loginUser.Id, userRoles));
                }

                if (string.IsNullOrEmpty(model.Password))
                {
                    if (currentUser.UserName.ToLower() == model.Username.ToLower())
                    {
                        return new Response<string>(await GenerateJwtToken(loginUser, User.Claims.FirstOrDefault(c => c.Type == "impersonatedBy")?.Value, userRoles));
                    }

                    if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian))
                    {
                        _giraf._logger.LogInformation("Guardian attempted to sign in as Citizen");
                        return await AttemptRoleLoginTokenAsync(currentUser, model.Username, GirafRole.Citizen);
                    }
                    else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department))
                    {
                        _giraf._logger.LogInformation("Department attempted to sign in as Guardian");
                        return await AttemptRoleLoginTokenAsync(currentUser, model.Username, GirafRole.Guardian);
                    }
                    else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Citizen))
                    {
                        if (await _giraf._userManager.IsInRoleAsync(loginUser, GirafRole.Guardian))
                            return new ErrorResponse<string>(ErrorCode.InvalidCredentials);
                    }
                }
            }
            return new ErrorResponse<string>(ErrorCode.InvalidCredentials);
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
            var loginUser = await _giraf.LoadByNameAsync(username);

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
            var doesUserAlreadyExist = (await _giraf.LoadByNameAsync(model.Username) != null);

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
