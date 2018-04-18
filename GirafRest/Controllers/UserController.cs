using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GirafRest.Models;
using GirafRest.Services;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using System;
using GirafRest.Extensions;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The user controller allows the user to change his information as well as add and remove applications
    /// and resources to users.
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class UserController : Controller
    {
        private const string IMAGE_TYPE_PNG = "image/png";
        private const string IMAGE_TYPE_JPEG = "image/jpeg";
        private const int IMAGE_CONTENT_TYPE_DEFINITION = 25;
        /// <summary>
        /// An email sender that can be used to send emails to users that have lost their password.
        /// </summary>
        private readonly IEmailService _emailSender;
        /// <summary>
        /// A reference to GirafService, that defines common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        /// <summary>
        /// A reference to the role manager for the project.
        /// </summary>
        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        /// <summary>
        /// Constructor for the User-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="emailSender">A reference to the emailservice.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        public UserController(
            IGirafService giraf,
          IEmailService emailSender,
          ILoggerFactory loggerFactory,
          RoleManager<GirafRole> roleManager, 
            IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("User");
            _emailSender = emailSender;
            _roleManager = roleManager;
            _authentication = authentication;
        }

        /// <summary>
        /// Returns currently logged in users username
        /// </summary>
        /// <returns>
        /// Reponse with username.
        /// UnAuthorized if user is not logged in.
        /// </returns>
        [HttpGet("username")]
        public async Task<Response<string>> Username()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return new ErrorResponse<string>(ErrorCode.NotAuthorized);

            return new Response<string>(user.UserName);
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>
        /// Data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged in user is not authorized to see user.
        ///</returns>
        [HttpGet("{username}")]
        public async Task<Response<GirafUserDTO>> GetUser(string username)
        {
            //Declare needed variables
            GirafUser user;

            //Check if the caller has supplied a query, find the user with the given name if so,
            //else find the user with the given username.
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");

            //First attempt to fetch the user and check that he exists
            user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to see the user.
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);
            // if we are trying to login as the same user simply return the launchersettigns
            if (currentUser.Id == user.Id)
                return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
            if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian) ||
                await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department))
            {
                //Check if the guardian is in the same department as the user
                if (user.DepartmentKey != currentUser.DepartmentKey)
                    //We do not reveal if a user with the given username exists
                    return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);
            }
            else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser))
            {
                //No additional checks required, simply skip to Ok.
            }
            else
                //We do not reveal if a user with the given username exists
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // Get the roles the user is associated with

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>
        /// Data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged in user is not authorized to see user.
        ///</returns>
        [HttpGet("{id}/settings")]
        public async Task<Response<LauncherOptionsDTO>> GetSettings(string id)
        {
            //Declare needed variables
            GirafUser user;

            //Check if the caller has supplied a query, find the user with the given name if so,
            //else find the user with the given username.
            if (string.IsNullOrEmpty(id))
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.MissingProperties, "id");

            //First attempt to fetch the user and check that he exists
            user = _giraf._context.Users.Include(u => u.Settings).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.UserNotFound);

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to edit the settings.
            var authUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            GirafRoles authRole = await _roleManager.findUserRole(_giraf._userManager, authUser);
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user); 

            var errorCode = _authentication.CheckUserAccess(authUser, authRole, user, userRole);
            if (errorCode.HasValue)
                new ErrorResponse<LauncherOptionsDTO>(ErrorCode.NotAuthorized);

            return new Response<LauncherOptionsDTO>(new LauncherOptionsDTO(user.Settings));
        }

        /// <summary>
        /// Get information about the logged in user.
        /// </summary>
        /// <returns>
        /// Information about the logged in user
        /// </returns>
        [HttpGet("")]
        public async Task<Response<GirafUserDTO>> GetUser()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Updates username and screenname for the current authenticated user.
        /// </summary>
        /// <param name="userDTO">A DTO containing ALL the new information for the given user.</param>
        /// <returns>
        /// MissingProperties if username or screenname is null
        /// If succesfull returns the DTO corresponding to the newly updates user.
        /// </returns>
        [HttpPatch("")]
        public async Task<Response<GirafUserDTO>> UpdateUser([FromBody] string Username, [FromBody] string ScreenName)
        {
            //Fetch the user
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            return await UpdateUser(user.Id, Username, ScreenName);
        }

        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="Username">Username.</param>
        /// <param name="ScreenName">Screen name.</param>
        /// <returns>
        /// MissingProperties if username or screenname is null
        /// UserNotFound if no user can be found for the specified id
        /// Not authorised if trying to update a user you do not have priveligies to update
        /// Succes response with the DTO for the updates user if all went smooth
        /// </returns>
        [HttpPatch("{id}")]
        public async Task<Response<GirafUserDTO>> UpdateUser(string id, [FromBody] string Username, [FromBody] string ScreenName)
        {
            if (Username == null && ScreenName == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);

            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check whether user with that username already exist that does dot have the same id
            if (_giraf._context.Users.Any(u => u.UserName == Username && u.Id != user.Id))
            {
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyExists);
            }

            // current authenticated user
            var authenticatedUser = await _giraf.LoadUserAsync(HttpContext.User);
            var authRole = await _roleManager.findUserRole(_giraf._userManager, authenticatedUser);

            // update fields if they are not null
            if (!String.IsNullOrEmpty(Username))
                user.UserName = Username;

            if (!String.IsNullOrEmpty(ScreenName))
                user.DisplayName = ScreenName;
            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            if (authenticatedUser.Id == user.Id){
                //Save changes and return the user with updated information.
                _giraf._context.Users.Update(user);
                await _giraf._context.SaveChangesAsync();
                return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
            }

            var errorCode = _authentication.CheckUserAccess(authenticatedUser, authRole, user, userRole);
            if (errorCode.HasValue)
                return new ErrorResponse<GirafUserDTO>(errorCode.Value);
                

            // save and return 
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync();
            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        #region UserIcon
        /// <summary>
        /// Allows retrieval of user icon by anyone since an usericon should be public
        /// </summary>
        /// <returns>
        /// UserNotFound if no user with that id
        /// UserHasNoIcon if trying to get a user that does not have an icon
        /// return ImageDTO if succes
        /// </returns>
        [HttpGet("{id}/icon")]
        public async Task<Response<ImageDTO>> GetUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);

            if (user == null)
                return new ErrorResponse<ImageDTO>(ErrorCode.UserNotFound);
                

            if (user.UserIcon == null)
                return new ErrorResponse<ImageDTO>(ErrorCode.UserHasNoIcon);

            // return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(picto.Image)), "image/png");

            return new Response<ImageDTO>(new ImageDTO(user.UserIcon));
        }

        /// <summary>
        /// Allows retrieval of user icon by anyone since an usericon should be public
        /// </summary>
        /// <returns>Ok on success.</returns>
        [HttpGet("{id}/icon/raw")]
        public async Task<IActionResult> GetRawUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            if (user.UserIcon == null)
                return NotFound();

            return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(user.UserIcon)), "image/png");
        }

        /// <summary>
        /// Allows the user to update his profile icon.
        /// </summary>
        /// <returns>
        /// UserNotFound if no user authenticated
        /// Missingproperties if no new image specified
        /// returns succes respons if no error occured
        /// </returns>
        [HttpPut("icon")]
        public async Task<Response> SetUserIcon()
        {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);

            if (usr == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);

            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION)
                return new ErrorResponse(ErrorCode.MissingProperties, "Image");

            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }


        /// <summary>
        /// Allows the user to delete his profile icon.
        /// </summary>
        /// <returns>
        /// UserHasNoIcon if trying to delete an icon that does not exist
        /// Succes response if no error
        /// </returns>
        [HttpDelete("icon")]
        public async Task<Response> DeleteUserIcon()
        {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr.UserIcon == null)
                return new ErrorResponse(ErrorCode.UserHasNoIcon);

            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }
        #endregion
        #region Not strictly necessary methods, but more efficient than a PUT to user, as they only update a single value

        /// <summary>
        /// Updates the display name of the current user.
        /// </summary>
        /// <param name="displayName">The new display name of the user.</param>
        /// <returns>MissingProperties if no display name was specified
        /// success response and the user if all went smooth.</returns>
        [HttpPut("display-name")]
        public async Task<Response<GirafUserDTO>> UpdateDisplayName([FromBody] string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "displayname");

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.DisplayName = displayName;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Adds a resource to the given user's list of resources.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="resourceIdDTO"></param>
        /// <returns>
        /// MissingProperties if username or resourceIdDTO is null
        /// UserNotFound if no user with the given username
        /// ResourceNotFound if we cannot find the ressource to add
        /// ResourceMustBePrivate if the resource is not PRIVATE
        /// NotAuthorized if we do not own the ressource
        /// UserAlreadyOwnsResource if trying to add a ressource we already own
        /// </returns>
        [HttpPost("{username}/resource")]
        public async Task<Response<GirafUserDTO>> AddUserResource(string username, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");

            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "resourceIdDTO");

            //Attempt to find the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);

            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.Pictograms
                .Where(pf => pf.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();

            if (resource == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceNotFound);

            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceMustBePrivate);

            //Check that the currently authenticated user owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, curUsr);
            if (!resourceOwnedByCaller)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            //Check if the target user already owns the resource
            if (user.Resources.Any(ur => ur.ResourceKey == resourceIdDTO.Id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyOwnsResource);

            //Create the relation and save changes.
            var userResource = new UserResource(user, resource);
            await _giraf._context.UserResources.AddAsync(userResource);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Deletes a resource with the specified id from the given user's list of resources.
        /// </summary>
        /// <param name="resourceIdDTO"></param>
        /// <returns>
        /// MissingProperties if resourceIdDTO is null
        /// ResourceNotFound if we cannot find the ressource to delete
        /// UserDoesNotOwnResource if authenticatedUser does not own ressource
        /// The GirafUserDTO if no errors.
        /// </returns>
        [HttpDelete("resource")]
        public async Task<Response<GirafUserDTO>> DeleteResource([FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check that valid parameters have been specified in the call
            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "resourceIdDTO");

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();
            if (resource == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceNotFound);

            //Check if the caller owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            if (curUsr == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            //Fetch the relationship from the database and check that it exists
            var relationship = await _giraf._context.UserResources
                .Where(ur => ur.ResourceKey == resource.Id && ur.OtherKey == curUsr.Id)
                .FirstOrDefaultAsync();
            if (relationship == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.UserDoesNotOwnResource);

            //Remove the resource - both from the user's list and the database
            curUsr.Resources.Remove(relationship);
            _giraf._context.UserResources.Remove(relationship);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, curUsr);

            //Return Ok and the user - the resource is now visible in user.Resources
            return new Response<GirafUserDTO>(new GirafUserDTO(curUsr, userRole));
        }

        /// <summary>
        /// Enables or disables launcher animations for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether launcher animations should be enabled or not.</param>
        /// <returns>Ok and a userDTO of the current user.</returns>
        [HttpPost("launcher_animations/{enabled}")]
        public async Task<Response<GirafUserDTO>> ToggleAnimations(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.displayLauncherAnimations = enabled;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Gets the citizens for the specific user corresponding to the provided username.
        /// </summary>
        /// <returns>
        /// MissingProperties if username is null
        /// UserHasNoCitizens is user has no citizens
        /// List of all citizens as DTO if ok
        /// </returns>
        /// <param name="username">Username.</param>
        [HttpGet("{username}/citizens")]
	[Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<List<UserNameDTO>>> GetCitizens(string username)
        {
            if (String.IsNullOrEmpty(username))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.MissingProperties, "username");
            var user = await _giraf.LoadByNameAsync(username);
            var citizens = new List<UserNameDTO>();

            foreach (var citizen in user.Citizens)
            {
                var templist = _giraf._context.Users.ToList();
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == citizen.CitizenId);
                citizens.Add(new UserNameDTO { UserId = girafUser.Id, UserName = girafUser.UserName });
            }

            if (!citizens.Any())
            {
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.UserHasNoCitizens);
            }   

            return new Response<List<UserNameDTO>>(citizens.ToList<UserNameDTO>());
        }

        /// <summary>
        /// Gets the guardians for the specific user corresponding to the provided username.
        /// </summary>
        /// <returns>
        /// MissingProperties if username is null
        /// UserHasNoGuardians is user has no guardians
        /// List of all guardians if ok </returns>
        /// <param name="username">Username.</param>
        [HttpGet("{username}/guardians")]
        [Authorize]
        public async Task<Response<List<UserNameDTO>>> GetGuardians(string username)
        {
            if (String.IsNullOrEmpty(username))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.MissingProperties, "username");
            var user = await _giraf.LoadByNameAsync(username);
            var guardians = new List<UserNameDTO>();
            foreach (var guardian in user.Guardians)
            {
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == guardian.GuardianId);
                guardians.Add(new UserNameDTO { UserId = girafUser.Id, UserName = girafUser.UserName });
            }

            if (!guardians.Any())
            {
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.UserHasNoGuardians);
            }

            return new Response<List<UserNameDTO>>(guardians);
        }


        /// <summary>
        /// Removes a user from its department.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <returns>
        /// UserNotFound if a user with the given username does not exsist.
        /// DepartmentNotFound if the user does not belong to any department.
        /// DepartmentDTO in its updated state if no problems occured.
        /// </returns>
        [HttpDelete("{username}/department")]
        public async Task<Response<DepartmentDTO>> RemoveDepartment(string username)
        {
            // TODO: Check that authorized user is a guardian of the department, is the department user or a superuser
            if(string.IsNullOrEmpty(username))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties, "username");

            var usr = await _giraf._context
                .Users
                .Include(u => u.Department)
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();
            
            if(usr == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
            
            var dep = await _giraf._context
                .Departments
                .Where(d => d.Key == usr.DepartmentKey)
                .Include(d => d.Members)
                .Include(d => d.Resources)
                .FirstOrDefaultAsync();
            
            if (dep == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            usr.DepartmentKey = null;
            _giraf._context.SaveChanges();
            
            return new Response<DepartmentDTO>(new DepartmentDTO(dep));
        }

        [HttpPost("guardian/{guardianId}/citizen/{citizenId}")]
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<GirafUserDTO>> AddGuardianCitizenRelationship(string guardianId, string citizenId)
        {
            var citizen = await _giraf._userManager.FindByIdAsync(citizenId);
            var guardian = await _giraf._userManager.FindByIdAsync(guardianId);

            if(citizen == null || guardian == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound, "User, either guardian or citizen, not found");

            if (String.IsNullOrEmpty(citizen.UserName) || String.IsNullOrEmpty(guardian.UserName))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");

            var citRole = _roleManager.findUserRole(_giraf._userManager, citizen).Result;
            var guaRole = _roleManager.findUserRole(_giraf._userManager, guardian).Result;

            if (citRole != GirafRoles.Citizen || guaRole != GirafRoles.Guardian)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.InvalidProperties, "Role error, either citizen is not a citizen or guardian is not a guardian");

            citizen.AddGuardian(guardian);

            return new Response<GirafUserDTO>(new GirafUserDTO(citizen, GirafRoles.Citizen));
        }

        /// <summary>
        /// Read the currently authorized user's settings object.
        /// </summary>
        /// <returns>The current user's settings if Ok </returns>
        [HttpGet("settings")]
        [Authorize]
        public async Task<Response<LauncherOptionsDTO>> ReadUserSettins()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if (user == null)
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.NotAuthorized);

            return new Response<LauncherOptionsDTO>(new LauncherOptionsDTO(user.Settings));
        }

        /// <summary>
        /// Updates the user settings.
        /// </summary>
        /// <returns>
        /// MissingProperties if options is null or some required fields is not set
        /// </returns>
        /// <param name="options">Options.</param>
        [HttpPatch("settings")]
        public async Task<Response<LauncherOptions>> UpdateUserSettings([FromBody] LauncherOptionsDTO options)
        {
            if (options == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, "options");
            
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);

            user.Settings = _giraf._context.Users.Where(u => u.Id == user.Id).Select(s => s.Settings).FirstOrDefault();

            if (user == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.NotAuthorized);
            if (!ModelState.IsValid)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());


            user.Settings.UpdateFrom(options);
            await _giraf._context.SaveChangesAsync();
            return new Response<LauncherOptions>(user.Settings);
        }


        /// <summary>
        /// Updates the user settings.
        /// </summary>
        /// <returns>
        /// MissingProperties if options is null or some required fields is not set
        /// </returns>
        /// <param name="options">Options.</param>
        [HttpPatch("{id}/settings")]
        [Authorize]
        public async Task<Response<LauncherOptions>> UpdateUserSettings(string id, [FromBody] LauncherOptionsDTO options)
        {
            if (!ModelState.IsValid)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());
            
            if (options == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, "options");

            var user =  _giraf._context.Users.Include(u => u.Settings).FirstOrDefault(u => u.Id == id);

            if (user == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.UserNotFound);

            if (user.Settings == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingSettings);

            var authUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            var authUserRole = await _roleManager.findUserRole(_giraf._userManager, authUser);
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            var errorCode = _authentication.CheckUserAccess(authUser, authUserRole, user, userRole);
            if (errorCode != null)
                return new ErrorResponse<LauncherOptions>(errorCode.Value); 
            
            user.Settings.UpdateFrom(options);
            await _giraf._context.SaveChangesAsync();
            return new Response<LauncherOptions>(user.Settings);
        }

        #endregion
        #region Helpers
        /// <summary>
        /// Attempts to update the users resources from the ids given in the collection.
        /// </summary>
        /// <param name="user">The user, whose resources should be updated.</param>
        /// <param name="resourceIds">The ids of the users new resources.</param>
        /// <returns></returns>
        private void updateResource(GirafUser user, ICollection<long> resourceIds)
        {
            //Check if the user has attempted to add a resource in the PUT request - throw an exception if so.
            foreach (var resourceId in resourceIds)
            {
                if (!user.Resources.Any(r => r.ResourceKey == resourceId))
                {
                    throw new InvalidOperationException("You may not add pictograms to a user by a PUT request. " +
                        "Please use a POST to user/resource instead");
                }
            }

            //Remove all the resources that are in the user's list, but not in the id-list
            foreach (var resource in user.Resources)
            {
                if (!resourceIds.Contains(resource.ResourceKey))
                    _giraf._context.Remove(resource);
            }
        }
        /// <summary>
        /// Attempts to update the user's list of week schedules.
        /// </summary>
        /// <param name="user">The user to update the week schedules of.</param>
        /// <param name="weekschedule">A list of DTOs for the user's new week schedules.</param>
        private async Task updateWeekAsync(GirafUser user, ICollection<WeekDTO> weekschedule)
        {
            //Run over the user's list of week schedules - delete those that are not in the list of DTOs
            foreach (var week in user.WeekSchedule)
            {
                if (!weekschedule.Any(w => w.Id == week.Id))
                    _giraf._context.Remove(week);
            }

            //Run over the list of DTOs - add those that are not in the user's list of weeks
            foreach (var week in weekschedule)
            {
                if (!user.WeekSchedule.Any(w => w.Id == week.Id))
                {
                    var newWeek = new Week(week);
                    await _giraf._context.Weeks.AddAsync(newWeek);
                    user.WeekSchedule.Add(newWeek);
                }
            }
        }
        /// <summary>
        /// Attempts to update the user's department from the given id.
        /// </summary>
        /// <param name="user">The user, whose department should be updated.</param>
        /// <param name="departmentId">The id of the user's new department.</param>
        private async Task updateDepartmentAsync(GirafUser user, long? departmentId)
        {
            if (departmentId == null)
            {
                user.Department.Members.Remove(user);
                user.Department = null;
                return;
            }

            user.DepartmentKey = (long)departmentId;
            var dep = await _giraf._context.Departments.Where(d => d.Key == departmentId).FirstOrDefaultAsync();
            user.Department = dep ?? throw new KeyNotFoundException("There is no department with the given id: " + departmentId);
        }

        private void updateGuardians(GirafUser user, List<GirafUserDTO> guardians)
        {
            if (guardians != null && guardians.Any())
            {
                // delete old guardians
                user.Guardians = new List<GuardianRelation>();
                var guardianUsers = new List<GuardianRelation>();
                foreach (var guardian in guardians)
                {
                    var gUser = _giraf._context.Users.FirstOrDefaultAsync(u => u.Id == guardian.Id).Result;

                    if (gUser != null)
                        user.AddGuardian(gUser);
                }
            }
        }

        private void updateCitizens(GirafUser user, List<GirafUserDTO> citizens)
        {
            if (citizens != null && citizens.Any())
            {
                // delete old citizens
                user.Citizens = new List<GuardianRelation>();
                var citizenUsers = new List<GuardianRelation>();
                foreach (var citizen in citizens)
                {
                    var cUser = _giraf._context.Users.FirstOrDefaultAsync(u => u.Id == citizen.Id).Result;

                    if (cUser != null)
                        user.AddCitizen(cUser);
                }
            }
        }

        /// <summary>
        /// Writes all errors found by identity to the logger.
        /// </summary>
        /// <param name="result">The result from Identity when executing user-related actions.</param>
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                _giraf._logger.LogError(error.Description);
            }
        }
        #endregion
    }
}
