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
          RoleManager<GirafRole> roleManager)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("User");
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Returns currently logged in users username
        /// </summary>
        /// <returns>
        /// Reponse with username.
        /// UnAuthorized if user is not logged in.
        /// </returns>
        [HttpGet("username")]
        [Authorize]
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
        [Authorize]
        public async Task<Response<GirafUserSimplifiedDTO>> GetUser(string username)
        {
            //Declare needed variables
            GirafUser user;

            //Check if the caller has supplied a query, find the user with the given name if so,
            //else find the user with the given username.
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "username");

            //First attempt to fetch the user and check that he exists
            user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to see the user.
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian) ||
                await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department))
            {
                //Check if the guardian is in the same department as the user
                if (user.DepartmentKey != currentUser.DepartmentKey)
                    //We do not reveal if a user with the given username exists
                    return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);
            }
            else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser))
            {
                //No additional checks required, simply skip to Ok.
            }
            else
                //We do not reveal if a user with the given username exists
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>
        /// Data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged in user is not authorized to see user.
        ///</returns>
        [HttpGet("{username}/settings")]
        [Authorize]
        public async Task<Response<LauncherOptionsDTO>> GetSettings(string username)
        {
            //Declare needed variables
            GirafUser user;

            //Check if the caller has supplied a query, find the user with the given name if so,
            //else find the user with the given username.
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.MissingProperties, "username");

            //First attempt to fetch the user and check that he exists
            user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.UserNotFound);

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to see the user.
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian) ||
                await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department))
            {
                //Check if the guardian is in the same department as the user
                if (user.DepartmentKey != currentUser.DepartmentKey)
                    //We do not reveal if a user with the given username exists
                    return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.UserNotFound);
            }
            else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser))
            {
                //No additional checks required, simply skip to Ok.
            }
            else
                //We do not reveal if a user with the given username exists
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.UserNotFound);

            return new Response<LauncherOptionsDTO>(new LauncherOptionsDTO(user.Settings));
        }

        /// <summary>
        /// Get information about the logged in user.
        /// </summary>
        /// <returns>
        /// Information about the logged in user
        /// </returns>
        [Authorize]
        [HttpGet("")]
        public async Task<Response<GirafUserSimplifiedDTO>> GetUser()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Updates all the information of the currently authenticated user with the information from the given DTO.
        /// </summary>
        /// <param name="userDTO">A DTO containing ALL the new information for the given user.</param>
        /// <returns>
        /// NotFound if the DTO contains either an invalid pictogram ID or an invalid week ID and
        /// OK if the user was updated succesfully.
        /// </returns>
        [HttpPut("")]
        public async Task<Response<GirafUserSimplifiedDTO>> UpdateUser([FromBody]GirafUserSimplifiedDTO userDTO)
        {
            if(userDTO == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties);
            
            //Fetch the user
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            
            if(user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);

            return await UpdateUser(user.Id, userDTO);
        }

        /// <summary>
        /// Updates all the information of the currently authenticated user with the information from the given DTO.
        /// </summary>
        /// <param name="id">The id of the user to update.</param>
        /// <param name="userDTO">A DTO containing ALL the new information for the given user.</param>
        /// <returns>
        /// NotFound if the DTO contains either an invalid pictogram ID or an invalid week ID and
        /// OK if the user was updated succesfully.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<Response<GirafUserSimplifiedDTO>> UpdateUser(string id, [FromBody]GirafUserSimplifiedDTO userDTO)
        {
            var userInfoBrief = await _giraf._userManager.FindByIdAsync(id);

            if(userInfoBrief == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);
            
            //Fetch the user
            var user = await _giraf.LoadByNameAsync(userInfoBrief.UserName);
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.NotFound);

            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(e => e.Errors.Count > 0)
                                  .SelectMany(e => e.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToArray());

            //Update all simple fields
            user.UserName = userDTO.Username;
            user.DisplayName = userDTO.ScreenName;

            //Save changes and return the user with updated information.
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        #region UserIcon
        /// <summary>
        /// Allows the user to retrieve his profile icon.
        /// </summary>
        /// <returns>Ok on success.</returns>
        [HttpGet("{id}/icon")]
        public async Task<Response<ImageDTO>> GetUserIcon(string id) 
        {
            // TODO:Check if request is authorized to get the users image
            GirafUser user = await _giraf._userManager.FindByIdAsync(id);

            if(user == null)
                return new ErrorResponse<ImageDTO>(ErrorCode.UserNotFound);
            
            if (user.UserIcon == null) 
                return new ErrorResponse<ImageDTO>(ErrorCode.UserHasNoIcon);

            // return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(picto.Image)), "image/png");
            
            return new Response<ImageDTO>(new ImageDTO(user.UserIcon));
        }

        /// <summary>
        /// Allows the user to retrieve his profile icon.
        /// </summary>
        /// <returns>Ok on success.</returns>
        [HttpGet("{id}/icon/raw")]
        public async Task<IActionResult> GetRawUserIcon(string id) 
        {
            // TODO:Check if request is authorized to get the users image
            GirafUser user = await _giraf._userManager.FindByIdAsync(id);

            if(user == null)
                return NotFound();
            
            if (user.UserIcon == null) 
                return NotFound();

            return File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(user.UserIcon)), "image/png");
        }
        
        /// <summary>
        /// Allows the user to update his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpPut("icon")]
        public async Task<Response<GirafUserSimplifiedDTO>> SetUserIcon() 
        {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            
            if(usr == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);
            
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            
            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION) 
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "Image");
            
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(usr, userRole));
        }


        /// <summary>
        /// Allows the user to delete his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpDelete("icon")]
        public async Task<Response<GirafUserSimplifiedDTO>> DeleteUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr.UserIcon == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserHasNoIcon);

            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(usr, userRole));
        }
        #endregion
        #region Not strictly necessary methods, but more efficient than a PUT to user, as they only update a single value

        /// <summary>
        /// Adds an application to the specified user's list of applications.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="application">Information on the new application to add, must be serialized
        /// and in the request body. Please specify ApplicationName and ApplicationPackage.</param>
        /// <returns>BadRequest in no application is specified,
        /// NotFound if no user with the given id exists or
        /// Ok and a serialized version of the user to whom the application was added.</returns>
        [HttpPost("applications/{username}")]
        public async Task<Response<GirafUserSimplifiedDTO>> AddApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check that an application has been specified
            if (string.IsNullOrEmpty(application?.ApplicationName) 
                || string.IsNullOrEmpty(application.ApplicationPackage))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "application");
            
            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());

            //Fetch the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound, "username");

            if (user.Settings.appsUserCanAccess.Any(aa => aa.ApplicationName.Equals(application.ApplicationName)))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserAlreadyHasAccess);

            //Add the application for the user to see
            user.Settings.appsUserCanAccess.Add(application);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }
        /// <summary>
        /// Delete an application from the given user's list of applications.
        /// </summary>
        /// <param name="username">The username of the user to delete the application from.</param>
        /// <param name="application">The application to delete (its ID is sufficient).</param>
        /// <returns>BadRequest if no application is specified,
        /// NotFound if no user or applications with the given ids exist
        /// or Ok and the user if everything went well.</returns>
        [HttpDelete("applications/{username}")]
        public async Task<Response<GirafUserSimplifiedDTO>> DeleteApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check if the caller has specified an application to remove
            if (application == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "application");

            //Fetch the user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound, "username");

            //Check if the given application was previously available to the user
            var app = user.Settings.appsUserCanAccess.FirstOrDefault(a => a.Id == application.Id);
            if (app == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.ApplicationNotFound, "application");

            //Remove it and save changes
            user.Settings.appsUserCanAccess.Remove(app);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Updates the display name of the current user.
        /// </summary>
        /// <param name="displayName">The new display name of the user.</param>
        /// <returns>BadRequest if no display name was specified or Ok and the user.</returns>
        [HttpPut("display-name")]
        public async Task<Response<GirafUserSimplifiedDTO>> UpdateDisplayName([FromBody] string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "displayname");

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.DisplayName = displayName;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Adds a resource to the given user's list of resources.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="resourceIdDTO"></param>
        /// <returns>
        /// BadRequest if either of the two ids are missing or the resource is not PRIVATE, NotFound
        /// if either the user or the resource does not exist or Ok if everything went well.
        /// </returns>
        [HttpPost("resource/{username}")]
        public async Task<Response<GirafUserSimplifiedDTO>> AddUserResource(string username, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "username");
            
            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "resourceIdDTO");
            
            //Attempt to find the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            
            if (user == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserNotFound);

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.Pictograms
                .Where(pf => pf.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();
            
            if (resource == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.ResourceNotFound);
            
            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.ResourceMustBePrivate);

            //Check that the currently authenticated user owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, curUsr);
            if (!resourceOwnedByCaller)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.NotAuthorized);

            //Check if the target user already owns the resource
            if (user.Resources.Any(ur => ur.ResourceKey == resourceIdDTO.Id))
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserAlreadyOwnsResource);

            //Create the relation and save changes.
            var userResource = new UserResource(user, resource);
            await _giraf._context.UserResources.AddAsync(userResource);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Deletes a resource with the specified id from the given user's list of resources.
        /// </summary>
        /// <param name="resourceIdDTO"></param>
        /// <returns>
        /// BadRequest if either of the two ids are missing or the resource is not PRIVATE, NotFound
        /// if either the user or the resource does not exist or Ok if everything went well.
        /// </returns>
        [HttpDelete("resource")]
        public async Task<Response<GirafUserSimplifiedDTO>> DeleteResource([FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check that valid parameters have been specified in the call
            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.MissingProperties, "resourceIdDTO");
            
            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();
            if (resource == null) return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.ResourceNotFound);

            //Check if the caller owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            if (curUsr == null)
                return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.NotAuthorized);

            //Fetch the relationship from the database and check that it exists
            var relationship = await _giraf._context.UserResources
                .Where(ur => ur.ResourceKey == resource.Id && ur.OtherKey == curUsr.Id)
                .FirstOrDefaultAsync();
            if (relationship == null) return new ErrorResponse<GirafUserSimplifiedDTO>(ErrorCode.UserDoesNotOwnResource);

            //Remove the resource - both from the user's list and the database
            curUsr.Resources.Remove(relationship);
            _giraf._context.UserResources.Remove(relationship);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, curUsr);

            //Return Ok and the user - the resource is now visible in user.Resources
            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(curUsr, userRole));
        }
        
        /// <summary>
        /// Enables or disables grayscale mode for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether grayscale should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("grayscale/{enabled}")]
        public async Task<Response<GirafUserSimplifiedDTO>> ToggleGrayscale(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.UseGrayscale = enabled;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Enables or disables launcher animations for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether launcher animations should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("launcher_animations/{enabled}")]
        public async Task<Response<GirafUserSimplifiedDTO>> ToggleAnimations(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.DisplayLauncherAnimations = enabled;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserSimplifiedDTO>(new GirafUserSimplifiedDTO(user, userRole));
        }

        /// <summary>
        /// Gets the citizens for the specific user corresponding to the provided username.
        /// </summary>
        /// <returns>The citizens.</returns>
        /// <param name="username">Username.</param>
        [HttpGet("getCitizens/{username}")]
        public async Task<Response<List<GirafUserSimplifiedDTO>>> GetCitizens(string username)
        {
            if (username == null)
                return new ErrorResponse<List<GirafUserSimplifiedDTO>>(ErrorCode.MissingProperties, "username");
            var user = await _giraf.LoadByNameAsync(username);
            var citizens = new List<GirafUserSimplifiedDTO>();

            foreach (var citizen in user.Citizens)
            {
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == citizen.CitizenId);
                citizens.Add(new GirafUserSimplifiedDTO(girafUser, GirafRoles.Citizen));
            }

            if (!citizens.Any())
            {
                return new ErrorResponse<List<GirafUserSimplifiedDTO>>(ErrorCode.UserHasNoCitizens);
            }

            return new Response<List<GirafUserSimplifiedDTO>>(citizens);
        }

        /// <summary>
        /// Gets the guardians for the specific user corresponding to the provided username.
        /// </summary>
        /// <returns>The guardians.</returns>
        /// <param name="username">Username.</param>
        [HttpGet("getGuardians/{username}")]
        [Authorize]
        public async Task<Response<List<GirafUserSimplifiedDTO>>> GetGuardians(string username)
        {
            if (username == null)
                return new ErrorResponse<List<GirafUserSimplifiedDTO>>(ErrorCode.MissingProperties, "username");
            var user = await _giraf.LoadByNameAsync(username);
            var guardians = new List<GirafUserSimplifiedDTO>();
            foreach (var guardian in user.Guardians)
            {
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == guardian.GuardianId);
                guardians.Add(new GirafUserSimplifiedDTO(girafUser, GirafRoles.Guardian));
            }

            if (!guardians.Any())
            {
                return new ErrorResponse<List<GirafUserSimplifiedDTO>>(ErrorCode.UserHasNoGuardians);
            }

            return new Response<List<GirafUserSimplifiedDTO>>(guardians);
        }

        /// <summary>
        /// Read the currently authorized user's settings object.
        /// </summary>
        /// <returns>The current user's settings.</returns>
        [HttpGet("settings")]
        [Authorize]
        public async Task<Response<LauncherOptionsDTO>> ReadUserSettins () {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if (user == null)
                return new ErrorResponse<LauncherOptionsDTO>(ErrorCode.NotAuthorized);

            return new Response<LauncherOptionsDTO>(new LauncherOptionsDTO(user.Settings));    
        }

        [HttpPut("settings")]
        [Authorize]
        public async Task<Response<LauncherOptions>> UpdateUserSettings ([FromBody] LauncherOptionsDTO options) {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if (user == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.NotAuthorized);
            if (options == null)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, "options");
            if (!ModelState.IsValid)
                return new ErrorResponse<LauncherOptions>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());

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
                if(!resourceIds.Contains(resource.ResourceKey))
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
                if(!user.WeekSchedule.Any(w => w.Id == week.Id))
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
            if (departmentId == null) {
                user.Department.Members.Remove(user);
                user.Department = null;
                return;
            }

            user.DepartmentKey = (long) departmentId;
            var dep = await _giraf._context.Departments.Where(d => d.Key == departmentId).FirstOrDefaultAsync();
            user.Department = dep ?? throw new KeyNotFoundException("There is no department with the given id: " + departmentId);
        }

        private void updateGuardians(GirafUser user, List<GirafUserSimplifiedDTO> guardians)
        {
            if(guardians != null && guardians.Any()){
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

        private void updateCitizens(GirafUser user, List<GirafUserSimplifiedDTO> citizens)
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
