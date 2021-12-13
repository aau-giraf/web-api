using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.DTOs.AccountDTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages accounts such as login, sign up, etc.
    /// </summary>
    [Authorize]
    [Route("v2/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<GirafUser> _signInManager;

        private readonly IGirafService _giraf;

        private readonly IOptions<JwtConfig> _configuration;

        private readonly IAuthenticationService _authentication;

        private readonly IGirafUserRepository _userRepository;

        private readonly IDepartmentRepository _departmentRepository;

        private readonly IGirafRoleRepository _girafRoleRepository;
        /// <summary>
        /// Constructor for AccountController
        /// </summary>
        /// <param name="signInManager">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="giraf">Service Injection</param>
        /// <param name="configuration">Service Injection</param>
        /// <param name="userRepository">Service Injection</param>
        /// <param name="departmentRepository">Service Injection</param>
        /// <param name="girafRoleRepository">Service Injection</param>
        public AccountController(
            SignInManager<GirafUser> signInManager,
            ILoggerFactory loggerFactory,
            IGirafService giraf,
            IOptions<JwtConfig> configuration,
            IGirafUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IGirafRoleRepository girafRoleRepository
        )
        {
            _signInManager = signInManager;
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Account");
            _configuration = configuration;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _girafRoleRepository = girafRoleRepository;
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
            
            // check that the caller has suplied password in the request
            if (string.IsNullOrEmpty(model.Password))
                return Unauthorized(new ErrorResponse(
                    ErrorCode.MissingProperties, "Missing password"));
            
            // check that the username exists in the database
            if (!_userRepository.ExistsUsername(model.Username))
                return Unauthorized(new ErrorResponse(ErrorCode.InvalidCredentials, "Invalid credentials"));

            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, true, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new ErrorResponse(ErrorCode.InvalidCredentials, "Invalid Credentials"));

            var loginUser = await _userRepository.GetUserByUsername(model.Username);
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

            if (String.IsNullOrEmpty(model.Username) || String.IsNullOrEmpty(model.Password) || String.IsNullOrEmpty(model.DisplayName))
                return BadRequest(new ErrorResponse(ErrorCode.InvalidCredentials, "Missing username, password or displayName"));

            var UserRoleStr = GirafRoleFromEnumToString(model.Role);
            if (UserRoleStr == null)
                return BadRequest(new ErrorResponse(ErrorCode.RoleNotFound, "The provided role is not valid"));

            if (_userRepository.ExistsUsername(model.Username))
                return Conflict(new ErrorResponse(ErrorCode.UserAlreadyExists, "User already exists", "A user with the given username already exists"));

            if (model.DepartmentId == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Did not find any Department ID",
                    "No Id found")); 
            Department department = _departmentRepository.GetDepartmentById((long)model.DepartmentId);

            // Check that the department with the specified id exists
            if (department == null)
                return BadRequest(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found", "A department with the given id could not be found"));

            //Create a new user with the supplied information
            var user = new GirafUser(model.Username, model.DisplayName, department, model.Role);

            var result  = await _signInManager.UserManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                if (department != null)
                {
                    if (model.Role == GirafRoles.Citizen)
                        AddGuardiansToCitizens(user);
                    else if (model.Role == GirafRoles.Guardian)
                        AddCitizensToGuardian(user);
                    // save changes
                    _departmentRepository.Save();
                }
                await _signInManager.UserManager.AddToRoleAsync(user, UserRoleStr);
                await _signInManager.SignInAsync(user, isPersistent: true);

                return Created("/v1/user/" + user.Id, new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, model.Role)));
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, "Something went wrong when creating user"));
        }

        /// <summary>
        /// Allows the user to change his password if they know their old password.
        /// </summary>
        /// <param name="userId">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <param name="model">A reference to <see cref="ChangePasswordDTO"/></param>
        /// <returns>
        /// Empty Response on success. Else: Missingproperties, PasswordNotUpdated or UserNotFound
        /// </returns>
        [HttpPut("password/{userId}")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> ChangePasswordByOldPassword(string userId, [FromBody] ChangePasswordDTO model)
        {
            var user = _userRepository.Get(userId);
            
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            if (model == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing model"));
            
            if (model.OldPassword == null || model.NewPassword == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing old password or new password"));
            
            var result =  await _signInManager.UserManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.PasswordNotUpdated, "Password was not updated"));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(new SuccessResponse("Password was updated"));
        }

        /// <summary>
        /// Allows a user to set a new password if they forgot theirs.
        /// </summary>
        /// <param name="userId">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <param name="model">All information needed to set the password in a ResetPasswordDTO, i.e. password and reset token.</param>
        /// <returns>
        /// Empty Response on success. 
        /// UserNotFound if invalid user id was suplied
        /// /// MissingProperties if there was missing properties
        /// </returns>
        [HttpPost("password/{userId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangePasswordByToken(string userId, [FromBody] ResetPasswordDTO model)
        {
            var user = _userRepository.Get(userId);
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
        /// <param name="userId">References the User, changing passwords. <see cref="GirafUser"/></param>
        /// <returns>
        /// Return the password reset token on success. 
        /// UserNotFound if invalid user id was suplied
        /// NotAuthorized if the  currently logged in user is not allowed to change the given users password
        /// </returns>
        [HttpGet("password-reset-token/{userId}")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetPasswordResetToken(string userId)
        {
            var user = _userRepository.Get(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            var result = await _giraf._userManager.GeneratePasswordResetTokenAsync(user);
            return Ok(new SuccessResponse(result));
        }

        /// <summary>
        /// Deletes the user with the given id
        /// </summary>
        /// <param name="userId">id for identifying the given <see cref="GirafUser"/> to be deleted</param>
        /// <returns>Empty response on success else UserNotFound or NotAuthorized</returns>
        [HttpDelete("user/{userId}")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUser(string userId)
        {
            var user = await _userRepository.GetUserWithId(userId);
            
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            _userRepository.Remove(user);
            _userRepository.Save();

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

        private void AddGuardiansToCitizens(GirafUser citizen)
        {
            var guardians = _girafRoleRepository.GetAllGuardians();
            var guardiansInDepartment = _userRepository.GetUsersInDepartment((long)citizen.DepartmentKey, guardians);
            foreach (var guardian in guardiansInDepartment)
            {
                citizen.AddGuardian(guardian);
            }
        }

        private void AddCitizensToGuardian(GirafUser guardian)
        {
            // Add a relation to all the newly created guardians citizens
            var citizens = _girafRoleRepository.GetAllCitizens();
            var citizensInDepartment = _userRepository.GetUsersInDepartment((long)guardian.DepartmentKey, citizens);
            foreach (var citizen in citizensInDepartment)
            {
                guardian.AddCitizen(citizen);
            }
        }
    }
}

