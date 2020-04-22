﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GirafRest.Models;
using GirafRest.Services;
using GirafRest.Models.DTOs.AccountDTOs;
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
using Microsoft.AspNetCore.Http;


namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages accounts such as login, sign up, etc.
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<GirafUser> _signInManager;

        private readonly IGirafService _giraf;

        private readonly IOptions<JwtConfig> _configuration;

        private readonly IAuthenticationService _authentication;

        /// <summary>
        /// Constructor for AccountController
        /// </summary>
        /// <param name="signInManager">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="giraf">Service Injection</param>
        /// <param name="configuration">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
        public AccountController(
            SignInManager<GirafUser> signInManager,
            ILoggerFactory loggerFactory,
            IGirafService giraf,
            IOptions<JwtConfig> configuration,
            IAuthenticationService authentication)
        {
            _signInManager = signInManager;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
            _configuration = configuration;
            _authentication = authentication;
        }

        /// <summary>
        /// This endpoint allows the user to sign in to his/her account by providing valid username and password.
        /// </summary>
        /// <param name="model">A <see cref="LoginDTO"/> i.e. a json object with username and password</param>
        /// <returns>
        /// JwtToken if credentials are valid else Errorcode: MissingProperties or InvalidCredentials
        /// </returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Login([FromBody]LoginDTO model)
        {
            if (model == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing model"));

            //Check that the caller has supplied username in the request
            if (string.IsNullOrEmpty(model.Username))
                return Unauthorized(new ErrorResponse(
                    ErrorCode.MissingProperties, "Missing username"));

            if (string.IsNullOrEmpty(model.Password))
                return Unauthorized(new ErrorResponse(
                    ErrorCode.MissingProperties, "Missing password"));

            if (!(_giraf._context.Users.Any(u => u.UserName == model.Username)))
                return Unauthorized(new ErrorResponse(ErrorCode.InvalidCredentials, "Invalid credentials" ));

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new ErrorResponse(ErrorCode.InvalidCredentials, "Invalid Credentials"));

            var loginUser = _giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username);
            return Ok(new SuccessResponse(await GenerateJwtToken(loginUser)));
        }

        /// <summary>
        /// Register a new user in the REST-API
        /// </summary>
        /// <param name="model">A reference to a <see cref="RegisterDTO"/> i.e. a json string containing;
        /// Username, Password, DisplayName, departmentId and Role.</param>
        /// <returns>
        /// Response with a GirafUserDTO for the registered user One of the following Error codes:
        ///  Missingproperties, InvalidCredentials, RoleNotFound, NotAuthorised, UserAlreadyExist, DepartmentNotFound,
        ///  Error
        /// </returns>
        [HttpPost("register")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Register([FromBody] RegisterDTO model)
        {
            if (model == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing model"));
            //Check that all the necesarry data has been supplied
            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Model is invalid"));

            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing username or password"));

            var UserRoleStr = GirafRoleFromEnumToString(model.Role);
            if (UserRoleStr == null)
                return BadRequest(new ErrorResponse(ErrorCode.RoleNotFound, "The provided role is not valid"));

            // check that authenticated user has the right to add user for the given department
            // else all guardians, deps and admin roles can create user that does not belong to a dep
            if (model.DepartmentId != null)
            {
                if (!(await _authentication.HasRegisterUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User),
                                                            model.Role, model.DepartmentId.Value)))
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User has no rights", 
                        "The authenticated user does not have the rights to add user for the given department"));
            }

            var doesUserAlreadyExist = (_giraf._context.Users.FirstOrDefault(u => u.UserName == model.Username) != null);


            if (doesUserAlreadyExist)
                return Conflict(new ErrorResponse(ErrorCode.UserAlreadyExists, "User already exists", "A user with the given username already exists"));

            Department department = await _giraf._context.Departments.Where(dep => dep.Key == model.DepartmentId).FirstOrDefaultAsync();

            // Check that the department with the specified id exists
            if (department == null && model.DepartmentId != null)
                return BadRequest(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found", "A department with the given id could not be found"));

            //Create a new user with the supplied information
            var user = new GirafUser(model.Username, department, model.Role);
            if (model.DisplayName == null)
            {
                user.DisplayName = model.Username;
            }
            else
            {
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

                return Created(Request.Host + "/v1/user/" + user.Id, new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, model.Role)));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, "Something went wrong when creating user"));
        }

        /// <summary>
        /// Allows the user to change his password if they know their old password.
        /// </summary>
        /// <param name="id">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <param name="model">A reference to <see cref="ChangePasswordDTO"/></param>
        /// <returns>
        /// Empty Response on success. Else: Missingproperties, PasswordNotUpdated or UserNotFound
        /// </returns>
        [HttpPut("/v1/User/{id}/Account/password")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePasswordByOldPassword(string id, [FromBody] ChangePasswordDTO model)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            if (model == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing model"));
            if (model.OldPassword == null || model.NewPassword == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing old password or new password"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "You do not have permission to edit this user"));

            var result = await _giraf._userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded) {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.PasswordNotUpdated, "Password was not updated"));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            _giraf._logger.LogInformation("User changed their password successfully.");
            return Ok(new SuccessResponse("Password was updated"));
        }

        /// <summary>
        /// Allows a user to set a new password if they forgot theirs.
        /// </summary>
        /// <param name="id">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <param name="model">All information needed to set the password in a ResetPasswordDTO, i.e. password and reset token.</param>
        /// <returns>
        /// Empty Response on success. 
        /// UserNotFound if invalid user id was suplied
        /// /// MissingProperties if there was missing properties
        /// </returns>
        [HttpPost("/v1/User/{id}/Account/password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangePasswordByToken(string id, [FromBody] ResetPasswordDTO model)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User was not found"));
            if (model == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing model"));
            if (model.Token == null || model.Password == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing token or password"));

            var result = await _giraf._userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
                return Unauthorized(new ErrorResponse(ErrorCode.InvalidProperties, "Invalid token"));

            await _signInManager.SignInAsync(user, isPersistent: false);
            _giraf._logger.LogInformation("User changed their password successfully.");
            return Ok(new SuccessResponse("User password changed succesfully"));
        }

        /// <summary>
        /// Allows the user to get a password reset token for a given user
        /// </summary>
        /// <param name="id">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <returns>
        /// Return the password reset token on success. 
        /// UserNotFound if invalid user id was suplied
        /// NotAuthorized if the  currently logged in user is not allowed to change the given users password
        /// </returns>
        [HttpGet("/v1/User/{id}/Account/password-reset-token")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetPasswordResetToken(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return Unauthorized(new ErrorResponse(ErrorCode.NotAuthorized, "Unauthorized"));

            var result = await _giraf._userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new SuccessResponse(result));
        }

        /// <summary>
        /// Deletes the user with the given id
        /// </summary>
        /// <param name="userId">id for identifying the given <see cref="GirafUser"/> to be deleted</param>
        /// <returns>Empty response on success else UserNotFound or NotAuthorized</returns>
        [HttpDelete("/v1/Account/user/{userId}")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // tjek om man kan slette sig selv, før jeg kan bruge hasreaduseraccess (sig hvis logged in id = userid så fejl)
            // A user cannot delete himself/herself
            var authenticatedUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (authenticatedUser == null || (authenticatedUser.Id == userId))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "Permission error"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have rights"));

            var result = _giraf._context.Users.Remove(user);
            _giraf._context.SaveChanges();

            return Ok(new SuccessResponse("User deleted"));
        }

        /// <summary>
        /// Gets roles s.t we can get role from payload 
        /// </summary>
        /// <param name="user"><see cref="GirafUser"/> to get claims for</param>
        /// <returns>The role claims for the given user</returns>
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
        /// <param name="user">Which <see cref="GirafUser"/> to generate the token for</param>
        /// <returns>
        /// JWT Token as a string
        /// </returns>
        private async Task<string> GenerateJwtToken(GirafUser user)
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

        /// <summary>
        /// Simple helper method for converting a role as enum to a role as string
        /// </summary>
        /// <returns>The role as a string</returns>
        /// <param name="role">A given role as enum that should be converted to a string</param>
        private string GirafRoleFromEnumToString(GirafRoles role)
        {
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

        private void AddGuardiansToCitizens(GirafUser user)
        {

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
