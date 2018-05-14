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
        /// reference to the authenticationservice which provides commong authentication checks
        /// </summary>
        private readonly IAuthenticationService _authentication;

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
            ILoggerFactory loggerFactory,
            IGirafService giraf,
            IOptions<JwtConfig> configuration,
            RoleManager<GirafRole> roleManager,
            IAuthenticationService authentication)
        {
            _signInManager = signInManager;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
            _configuration = configuration;
            _roleManager = roleManager;
            _authentication = authentication;
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

            if (string.IsNullOrEmpty(model.Password))
                return new ErrorResponse<string>(ErrorCode.MissingProperties, "password");

            if (!(_giraf._context.Users.Any(u => u.UserName == model.Username)))
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials); 

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return new ErrorResponse<string>(ErrorCode.InvalidCredentials);

            var loginUser = _giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username);
            var userRoles = await _roleManager.findUserRole(_giraf._userManager, loginUser);
            return new Response<string>(await GenerateJwtToken(loginUser, userRoles));

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
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        public async Task<Response<GirafUserDTO>> Register([FromBody] RegisterDTO model)
        {
            if(model == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);
            //Check that all the necesarry data has been supplied
            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);

            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.InvalidCredentials);

            var UserRoleStr = GirafRoleFromEnumToString(model.Role);
            if (UserRoleStr == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.RoleNotFound);

            // check that authenticated user has the right to add user for the given department
            // else all guardians, deps and admin roles can create user that does not belong to a dep
            if(model.DepartmentId != null){
                if(!(await _authentication.HasRegisterUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User),
                                                            model.Role, model.DepartmentId.Value)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            }

            var doesUserAlreadyExist = (_giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username) != null);

            if (doesUserAlreadyExist)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyExists);

            Department department = await _giraf._context.Departments.Where(dep => dep.Key == model.DepartmentId).FirstOrDefaultAsync();

            // Check that the department with the specified id exists
            if (department == null && model.DepartmentId != null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.DepartmentNotFound);

            //Create a new user with the supplied information
            var user = new GirafUser (model.Username, department);
            if (model.DisplayName == null){
                user.DisplayName = model.Username;
            }
            else{
                user.DisplayName = model.DisplayName;
            }
            var result = await _giraf._userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (department != null)
                {
                    if (model.Role == GirafRoles.Citizen)
                        AddGuardiansToCitizens(user);
                    else if (model.Role == GirafRoles.Guardian)
                        AddCitizensToGuardian(user);
                    // save changes
                    await _giraf._context.SaveChangesAsync();
                }
                await _giraf._userManager.AddToRoleAsync(user, UserRoleStr);
                await _signInManager.SignInAsync(user, isPersistent: true);
                _giraf._logger.LogInformation("User created a new account with password.");

                return new Response<GirafUserDTO>(new GirafUserDTO(user, model.Role));
            }

            return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);
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
        [HttpPost("/v1/User/{id}/Account/change-password")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        public async Task<Response> ChangePassword(string id,[FromBody] ChangePasswordDTO model)
        {
            var user =  _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);
            if (model == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword", "oldPassword");
            if (model.OldPassword == null || model.NewPassword == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "newPassword", "oldPassword");

            // check access rights
            if (!(await _authentication.HasReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            var result = await _giraf._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                return new ErrorResponse(ErrorCode.PasswordNotUpdated);

            await _signInManager.SignInAsync(user, isPersistent: false);
            _giraf._logger.LogInformation("User changed their password successfully.");
            return new Response();
        }

        [HttpDelete("/v1/Account/user/{userId}")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        public async Task<Response> DeleteUser(string userId)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            // tjek om man kan slette sig selv, før jeg kan bruge hasreaduseraccess (sig hvis logged in id = userid så fejl)

            // check access rights
            if (!(await _authentication.HasReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            var result = _giraf._context.Users.Remove(user);
            _giraf._context.SaveChanges();

            return new Response();
        }

        /// <summary>
        /// Gets roles s.t we can get role from payload 
        /// </summary>
        /// <returns>The role claims.</returns>
        /// <param name="user">User.</param>
        private async Task<List<Claim>> GetRoleClaims(GirafUser user)
        {
            var roleclaims = new List<Claim>();
            var userRoles = await _giraf._userManager.GetRolesAsync(user);
            roleclaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
            return roleclaims;
        }

        /// <summary>
        /// Generates a JSON Web Token Token (JwtToken) for a given user and role. Based on the method with the same name from https://github.com/jatarga/WebApiJwt/blob/master/Controllers/AccountController.cs
        /// </summary>
        /// <param name="user">Which user</param>
        /// <param name="roles">Which roles</param>
        /// <returns>
        /// The Token as a string
        /// </returns>
        private async Task<string> GenerateJwtToken(GirafUser user, GirafRoles roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("departmentId", user.DepartmentKey?.ToString() ?? ""),
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

        private string GirafRoleFromEnumToString(GirafRoles role){
            switch (role)
            {
                case GirafRoles.Citizen:
                    return GirafRole.Citizen;
                case GirafRoles.Guardian:
                    return GirafRole.Guardian;
                case GirafRoles.Department:
                    return GirafRole.Department;
                case GirafRoles.SuperUser:
                    return GirafRole.SuperUser;
                default:
                    return null;
            }
        }
        /// <summary>
        /// // Add a relation to all the newly created citizens guardians
        /// </summary>
        /// <param name="user">user to add relation for.</param>
        /// <param name="department">department the user belongs to.</param>
        private void AddGuardiansToCitizens(GirafUser user){
            
            var roleGuardianId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Guardian)
                                       .Select(c => c.Id).FirstOrDefault();
            var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleGuardianId)
                                .Select(r => r.UserId).Distinct();
            var guardians = _giraf._context.Users.Where(u => userIds.Any(ui => ui == u.Id)
                                                        && u.DepartmentKey == user.DepartmentKey).ToList();
            foreach (var guardian in guardians)
            {
                user.AddGuardian(guardian);
            }
        }

        private void AddCitizensToGuardian(GirafUser user)
        {
            // Add a relation to all the newly created guardians citizens
            var roleGuardianId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Citizen)
                                       .Select(c => c.Id).FirstOrDefault();
            var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleGuardianId)
                                .Select(r => r.UserId).Distinct();
            var citizens = _giraf._context.Users.Where(u => userIds.Any(ui => ui == u.Id)
                                                       && u.DepartmentKey == user.DepartmentKey).ToList();
            foreach (var citizen in citizens)
            {
                user.AddCitizen(citizen);
            }
        }

    }
}
