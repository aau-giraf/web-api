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
using GirafRest.Models.DTOs.UserDTOs;
using System.Collections.Generic;
using System.Reflection;

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
        /// <summary>
        /// An email sender that can be used to send emails to users that have lost their password.
        /// </summary>
        private readonly IEmailService _emailSender;
        /// <summary>
        /// A reference to GirafService, that defines common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;

        /// <summary>
        /// Constructor for the User-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="emailSender">A reference to the emailservice.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        public UserController(
            IGirafService giraf,
          IEmailService emailSender,
          ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("User");
            _emailSender = emailSender;
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>NotFound either if there is no user with the given username or the user is not authorized to see the user
        /// or Ok and a serialized version of the sought-after user.</returns>
        [HttpGet("{username}")]
        [Authorize]
        public async Task<IActionResult> GetUser(string username)
        {
            //Declare needed variables
            GirafUser user;

            //Check if the caller has supplied a query, find the user with the given name if so,
            //else find the user with the given username.
            if (string.IsNullOrEmpty(username))
                BadRequest("Please specify a username to search for.");

            //First attempt to fetch the user and check that he exists
            user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return NotFound();

            //Get the current user and check if he is a guardian in the same department as the user
            //or an Admin, in which cases the user is allowed to see the user.
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Guardian))
            {
                //Check if the guardian is in the same department as the user
                if (user.DepartmentKey != currentUser.DepartmentKey)
                    //We do not reveal if a user with the given username exists
                    return NotFound();
            }
            else if (await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.Admin))
            {
                //No additional checks required, simply skip to Ok.
            }
            else
                //We do not reveal if a user with the given username exists
                return NotFound();

            return Ok(new GirafUserDTO(user));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser ()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return NotFound();

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

            return Ok(new GirafUserDTO(user));
        }

        /// <summary>
        /// Updates all the information of the currently authenticated user with the information from the given DTO.
        /// </summary>
        /// <param name="userDTO">A DTO containing ALL the new information for the given user.</param>
        /// <returns>
        /// NotFound if the DTO contains either an invalid pictogram ID or an invalid week ID and
        /// OK if the user was updated succesfully.
        /// </returns>
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]GirafUserDTO userDTO)
        {
            //Fetch the user
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
                return NotFound("User not found!");

            // Check if DTO or its properties is null
            if (userDTO == null)
                return BadRequest("DTO must not be null!");
            foreach (var property in userDTO.GetType().GetProperties())
                if (property.GetValue(userDTO) == null)
                    if (property.Name == "settings" || property.Name == "username")
                        return BadRequest("Info in userDTO must be set!");

            //Update all simple fields
            user.Settings = userDTO.Settings;
            user.UserName = userDTO.Username;
            user.DisplayName = userDTO.DisplayName;
            user.UserIcon = userDTO.UserIcon;

            //Attempt to update all fields that require database access.
            try
            {
                await updateDepartmentAsync(user, userDTO.DepartmentKey);
                await updateResourceAsync(user, userDTO.Resources);
                await updateWeekAsync(user, userDTO.WeekScheduleIds);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }

            //Save changes and return the user with updated information.
            
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }
        #region UserIcon
        /// <summary>
        /// Allows the user to upload an icon for his profile.
        /// </summary>
        /// <returns>Ok if the upload was successful and BadRequest if not.</returns>
        [HttpPost("icon")]
        public async Task<IActionResult> CreateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon != null) return BadRequest("The user already has an icon - please PUT instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
        }
        /// <summary>
        /// Allows the user to update his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpPut("icon")]
        public async Task<IActionResult> UpdateUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(usr.UserIcon == null) return BadRequest("The user does not have an icon - please POST instead.");
            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);
            usr.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
        }
        /// <summary>
        /// Allows the user to delete his profile icon.
        /// </summary>
        /// <returns>Ok on success and BadRequest if the user already has an icon.</returns>
        [HttpDelete("icon")]
        public async Task<IActionResult> DeleteUserIcon() {
            var usr = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (usr.UserIcon == null)
                return BadRequest("The user does not have an icon to delete.");

            usr.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return Ok(new GirafUserDTO(usr));
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
        public async Task<IActionResult> AddApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check that an application has been specified
            if (application == null)
                return BadRequest("No application was specified in the request body.");
            if (application.ApplicationName == null || application.ApplicationPackage == null)
                return BadRequest("You need to specify both an application name and an application package.");

            //Fetch the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return NotFound($"There is no user with id: {username}");

            if (user.Settings.appsUserCanAccess.Where(aa => aa.ApplicationName.Equals(application.ApplicationName)).Any())
                return BadRequest("The user already has access to the given application.");

            //Add the application for the user to see
            user.Settings.appsUserCanAccess.Add(application);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
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
        public async Task<IActionResult> DeleteApplication(string username, [FromBody] ApplicationOption application)
        {
            //Check if the caller has specified an application to remove
            if (application == null)
                return BadRequest("No application was specified in the request body.");

            //Fetch the user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return NotFound($"There is no user with id: {user}");

            //Check if the given application was previously available to the user
            var app = user.Settings.appsUserCanAccess.Where(a => a.Id == application.Id).FirstOrDefault();
            if (app == null)
                return NotFound("The user did not have an ApplicationOption with id " + application.Id);

            //Remove it and save changes
            user.Settings.appsUserCanAccess.Remove(app);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        /// <summary>
        /// Updates the display name of the current user.
        /// </summary>
        /// <param name="displayName">The new display name of the user.</param>
        /// <returns>BadRequest if no display name was specified or Ok and the user.</returns>
        [HttpPut("display-name")]
        public async Task<IActionResult> UpdateDisplayName([FromBody] string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return BadRequest("You need to specify a new display name");

            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.DisplayName = displayName;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
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
        public async Task<IActionResult> AddUserResource(string username, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(username))
                return BadRequest("You need to specify an id of a user.");
            if (resourceIdDTO == null)
                return BadRequest("You need to specify a resourceId in the body of the request.");

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.Pictograms
                .Where(pf => pf.Id == resourceIdDTO.ResourceId)
                .FirstOrDefaultAsync();
            if (resource == null)
                return NotFound("The is no resource with id " + resourceIdDTO.ResourceId);
            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return BadRequest("Resources must be PRIVATE (2) in order for users to own them.");

            //Check that the currently authenticated user owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, curUsr);
            if (!resourceOwnedByCaller)
                return Unauthorized();

            //Attempt to find the target user and check that he exists
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null)
                return NotFound("There is no user with username " + username);

            //Check if the target user already owns the resource
            if (user.Resources.Where(ur => ur.ResourceKey == resourceIdDTO.ResourceId).Any())
                return BadRequest("The user already owns the resource.");

            //Create the relation and save changes.
            var userResource = new UserResource(user, resource);
            await _giraf._context.UserResources.AddAsync(userResource);
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        /// <summary>
        /// Deletes a resource with the specified id from the given user's list of resources.
        /// </summary>
        /// <param name="userId">The id of the user to add the resource to.</param>
        /// <param name="resourceId">The id of the resource to add.</param>
        /// <returns>
        /// BadRequest if either of the two ids are missing or the resource is not PRIVATE, NotFound
        /// if either the user or the resource does not exist or Ok if everything went well.
        /// </returns>
        [HttpDelete("resource/{username}")]
        public async Task<IActionResult> DeleteResource(string username, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check that valid parameters have been specified in the call
            if (resourceIdDTO == null)
                return BadRequest("The body of the request must contain a resourceId");
            if (string.IsNullOrEmpty(username))
                return BadRequest("Please specify a userId to add the pictogram to");

            //Fetch the user and check that it exists.
            var user = await _giraf.LoadByNameAsync(username);
            if (user == null) return NotFound($"There is no department with Id {username}.");
            
            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceIdDTO.ResourceId)
                .FirstOrDefaultAsync();
            if (resource == null) return NotFound($"There is no resource with id {resourceIdDTO.ResourceId}.");

            //Check if the caller owns the resource
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
            var resourceOwned = await _giraf.CheckPrivateOwnership(resource, curUsr);
            if (!resourceOwned) return Unauthorized();

            //Check if the user already owns the resource and remove if so.
            var drrelation = await _giraf._context.UserResources
                .Where(ur => ur.ResourceKey == resource.Id && ur.OtherKey == user.Id)
                .FirstOrDefaultAsync();
            if (drrelation == null) return BadRequest("The user does not own the given resource.");
            user.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the user - the resource is now visible in user.Resources
            return Ok(new GirafUserDTO(user));
        }
        
        /// <summary>
        /// Enables or disables grayscale mode for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether grayscale should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("grayscale/{enabled}")]
        public async Task<IActionResult> ToggleGrayscale(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.UseGrayscale = enabled;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }

        /// <summary>
        /// Enables or disables launcher animations for the currently authenticated user.
        /// </summary>
        /// <param name="enabled">A bool indicating whether launcher animations should be enabled or not.</param>
        /// <returns>Ok and a serialized version of the current user.</returns>
        [HttpPost("launcher_animations/{enabled}")]
        public async Task<IActionResult> ToggleAnimations(bool enabled)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            user.Settings.DisplayLauncherAnimations = enabled;
            await _giraf._context.SaveChangesAsync();
            return Ok(new GirafUserDTO(user));
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Attempts to update the users resources from the ids given in the collection.
        /// </summary>
        /// <param name="user">The user, whose resources should be updated.</param>
        /// <param name="resouceIds">The ids of the users new resources.</param>
        /// <returns></returns>
        private async Task updateResourceAsync(GirafUser user, ICollection<long> resouceIds)
        {
            user.Resources.Clear();
            foreach (var id in resouceIds)
            {
                var pict = await _giraf._context.Pictograms
                    .Where(p => p.Id == id)
                    .FirstOrDefaultAsync();
                if (pict == null) throw new KeyNotFoundException("There is no pictogram with the given id: " + id);
                user.Resources.Add(new UserResource(user, pict));
            }
        }
        /// <summary>
        /// Attempts to update the user's list of week schedules.
        /// </summary>
        /// <param name="user">The user to update the week schedules of.</param>
        /// <param name="weekscheduleIds">A list of ids for the user's new week schedules.</param>
        private async Task updateWeekAsync(GirafUser user, ICollection<long> weekscheduleIds)
        {
            foreach (var id in weekscheduleIds)
            {
                var week = await _giraf._context.Weeks
                    .Where(w => w.Id == id)
                    .FirstOrDefaultAsync();
                if (week == null) throw new KeyNotFoundException("There is no week with the given id: " + id);
                user.WeekSchedule.Add(week);
            }
            if (user.WeekSchedule != null) user.WeekSchedule.Clear();
        }
        /// <summary>
        /// Attempts to update the user's department from the given id.
        /// </summary>
        /// <param name="user">The user, whose department should be updated.</param>
        /// <param name="departmentId">The id of the user's new department.</param>
        private async Task updateDepartmentAsync(GirafUser user, long? departmentId)
        {
            if (departmentId == null)
                return;

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
