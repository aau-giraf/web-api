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
using System.Reflection;
using System;
using static GirafRest.Models.DTOs.GirafUserDTO;
using GirafRest.Extensions;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The user controller allows the user to change his information as well as add and remove applications
    /// and resources to users.
    /// </summary>
    [Authorize]
    [Route("[controller]")]
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
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>NotFound either if there is no user with the given username or the user is not authorized to see the user
        /// or Ok and a serialized version of the sought-after user.</returns>
        [HttpGet("{username}")]
        [Authorize]
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
                return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to see the user.
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian) ||
                await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Department))
            {
                //Check if the guardian is in the same department as the user
                if (user.DepartmentKey != currentUser.DepartmentKey)
                    //We do not reveal if a user with the given username exists
                    return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);
            }
            else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser))
            {
                //No additional checks required, simply skip to Ok.
            }
            else
                //We do not reveal if a user with the given username exists
                return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        [Authorize]
        [HttpGet("")]
        public async Task<Response<GirafUserDTO>> GetUser ()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotFound);

            if (await _giraf._userManager.IsInRoleAsync(user, GirafRole.Guardian))
            {
                var dep = await _giraf._context.Departments
                    .Where(d => d.Key == user.DepartmentKey)
                    .Include(d => d.Members)
                    .FirstOrDefaultAsync();
                user.GuardianOf = dep.Members.Where(m => _giraf._userManager.IsInRoleAsync(m, GirafRole.Citizen).Result).ToList();
                }
            else if (await _giraf._userManager.IsInRoleAsync(user, GirafRole.Department))
            {
                var dep = await _giraf._context.Departments
                    .Where(d => d.Key == user.DepartmentKey)
                    .Include(d => d.Members)
                    .FirstOrDefaultAsync();
                user.GuardianOf = dep.Members.Where(m => _giraf._userManager.IsInRoleAsync(m, GirafRole.Guardian).Result).ToList();
            }

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
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
        public async Task<Response<GirafUserDTO>> UpdateUser([FromBody]GirafUserDTO userDTO)
        {
            //Fetch the user
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            return await UpdateUser(user.Id, userDTO);
        }

        /// <summary>
        /// Patch authenticated user to be guardian of the citizen given in body
        /// </summary>
        [Authorize]
        [HttpPatch("add-Guardian-Ship-Of-Citizen")]
        public async Task<Response> AddGuardianShipOfCitizen([FromBody] GirafUserDTO citizenDTO)
        {
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            user = _giraf._context.Users.Include(u => u.GuardianOf).FirstOrDefault(x => x.Id == user.Id);
            if (citizenDTO == null) {
                return new ErrorResponse(ErrorCode.FormatError);      
            }
            if (citizenDTO.Role != GirafRoles.Citizen) {
                return new ErrorResponse(ErrorCode.RoleMustBeCitizin);
            }

            if(user.Id == citizenDTO.Id){
                return new ErrorResponse(ErrorCode.UserCannotBeGuardianOfYourself);
            }

            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            if(userRole != GirafRoles.Guardian){
                return new ErrorResponse(ErrorCode.UserMustBeGuardian);
            }

            var CurrentGuardian = await _giraf._context.Users.Include(u => u.GuardianOf).FirstOrDefaultAsync(x => x.GuardianOf.Any(y => y.Id == citizenDTO.Id));

            if(CurrentGuardian != null){
                return new ErrorResponse(ErrorCode.CitizinAlreadyHasGuardian);
            }

            var citizen = _giraf._context.Users?.FirstOrDefault(g => g.Id == citizenDTO.Id);

            if(user?.Department?.Key != citizen?.Department?.Key){
                return new ErrorResponse(ErrorCode.UserAndCitizinMustBeInSameDepartment);
            }

            user.GuardianOf.Add(citizen);
            var success = await _giraf._context.SaveChangesAsync();

            return new Response();  
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
        public async Task<Response<GirafUserDTO>> UpdateUser(string id, [FromBody]GirafUserDTO userDTO)
        {
            var usr = await _giraf._userManager.FindByIdAsync(id);

            //Fetch the user
            var user = await _giraf.LoadByNameAsync(usr.UserName);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotFound);

            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());

            //Update all simple fields
            user.Settings.UpdateFrom(userDTO.Settings);
            user.UserName = userDTO.Username;
            user.DisplayName = userDTO.ScreenName;
            user.UserIcon = userDTO.UserIcon;

            //Attempt to update all fields that require database access.
            try
            {
                await updateDepartmentAsync(user, userDTO.Department);
                updateResource(user, userDTO.Resources.Select(i => i.Id).ToList());
                await updateWeekAsync(user, userDTO.WeekScheduleIds);
            }
            catch (KeyNotFoundException)
            {
                return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);
            }
            catch (InvalidOperationException)
            {
                return new ErrorResponse<GirafUserDTO>(ErrorCode.Error);
            }

            //Save changes and return the user with updated information.
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }
        #region UserIcon
        /// <summary>
        /// Allows the user to upload an icon for his profile.
        /// </summary>
        /// <returns>Ok if the upload was successful and BadRequest if not.</returns>
        [Consumes(IMAGE_TYPE_PNG, IMAGE_TYPE_JPEG)]
        [HttpPost("icon")]
        public async Task<Response<GirafUserDTO>> CreateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            if (usr.UserIcon != null) return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyHasIconUsePut);
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION) return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "Image");
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);

            return new Response<GirafUserDTO>(new GirafUserDTO(usr, userRole));
        }
        /// <summary>
        /// Allows the user to update his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpPut("icon")]
        public async Task<Response<GirafUserDTO>> UpdateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr.UserIcon == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.UserHasNoIconUsePost);
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION) return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "Image");
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);

            return new Response<GirafUserDTO>(new GirafUserDTO(usr, userRole));
        }

        [HttpDelete("delete-Guardian-Ship-Of-Citizen/{id}")]
        public async Task<Response> DeleteGuardianShipOfCitizen(string id)
        {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);
            if(userRole != GirafRoles.Guardian){
                return new ErrorResponse(ErrorCode.UserMustBeGuardian);
            }
            var citizenToDelete = _giraf._context.Users.Include(x => x.GuardianOf).FirstOrDefault(u => u.Id == usr.Id).GuardianOf.FirstOrDefault(g => g.Id == id);
            if(citizenToDelete == null){
                return new ErrorResponse(ErrorCode.CitizinNotFound);
            }
            usr.GuardianOf.Remove(citizenToDelete);
            var success = _giraf._context.SaveChanges();
 
            return new Response();
        }


        /// <summary>
        /// Allows the user to delete his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpDelete("icon")]
        public async Task<Response<GirafUserDTO>> DeleteUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr.UserIcon == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserHasNoIcon);

            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, usr);

            return new Response<GirafUserDTO>(new GirafUserDTO(usr, userRole));
        }
        #endregion
        #region Not strictly necessary methods, but more efficient than a PUT to user, as they only update a single value
        /// <summary>
        /// Adds an application to the specified user's list of applications.
        /// </summary>
        /// <param name="userId">The id of the user to add the application for.</param>
        /// <param name="application">Information on the new application to add, must be serialized
        /// and in the request body. Please specify ApplicationName and ApplicationPackage.</param>
        /// <returns>BadRequest in no application is specified,
        /// NotFound if no user with the given id exists or
        /// Ok and a serialized version of the user to whom the application was added.</returns>
        [HttpPost("applications/{username}")]
        public async Task<Response<GirafUserDTO>> AddApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check that an application has been specified
            if (application == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "application");
            if (!ModelState.IsValid)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());

            //Fetch the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound, "username");

            if (user.Settings.appsUserCanAccess.Where(aa => aa.ApplicationName.Equals(application.ApplicationName)).Any())
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyHasAccess);

            //Add the application for the user to see
            user.Settings.appsUserCanAccess.Add(application);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
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
        public async Task<Response<GirafUserDTO>> DeleteApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check if the caller has specified an application to remove
            if (application == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "application");

            //Fetch the user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound, "username");

            //Check if the given application was previously available to the user
            var app = user.Settings.appsUserCanAccess.Where(a => a.Id == application.Id).FirstOrDefault();
            if (app == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.ApplicationNotFound, "application");

            //Remove it and save changes
            user.Settings.appsUserCanAccess.Remove(app);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Updates the display name of the current user.
        /// </summary>
        /// <param name="displayName">The new display name of the user.</param>
        /// <returns>BadRequest if no display name was specified or Ok and the user.</returns>
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
        /// <param name="userId">The id of the user to add it to.</param>
        /// <param name="resourceId">The id of the resource to add.</param>
        /// <returns>
        /// BadRequest if either of the two ids are missing or the resource is not PRIVATE, NotFound
        /// if either the user or the resource does not exist or Ok if everything went well.
        /// </returns>
        [HttpPost("resource/{username}")]
        public async Task<Response<GirafUserDTO>> AddUserResource(string username, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(username))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");
            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "resourceIdDTO");

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

            //Attempt to find the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            //Check if the target user already owns the resource
            if (user.Resources.Where(ur => ur.ResourceKey == resourceIdDTO.Id).Any())
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
        /// <param name="resourceId">The id of the resource to add.</param>
        /// <returns>
        /// BadRequest if either of the two ids are missing or the resource is not PRIVATE, NotFound
        /// if either the user or the resource does not exist or Ok if everything went well.
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
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, curUsr);

            //Return Ok and the user - the resource is now visible in user.Resources
            return new Response<GirafUserDTO>(new GirafUserDTO(curUsr, userRole));
        }
        
        /// <summary>
        /// Enables or disables grayscale mode for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether grayscale should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("grayscale/{enabled}")]
        public async Task<Response<GirafUserDTO>> ToggleGrayscale(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.UseGrayscale = enabled;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Enables or disables launcher animations for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether launcher animations should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("launcher_animations/{enabled}")]
        public async Task<Response<GirafUserDTO>> ToggleAnimations(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.DisplayLauncherAnimations = enabled;
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
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
        /// <param name="resouceIds">The ids of the users new resources.</param>
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
            if (dep == null)
                throw new KeyNotFoundException("There is no department with the given id: " + departmentId);
            user.Department = dep;
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
