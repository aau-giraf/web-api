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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages Users, settings etc.
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class UserController : Controller
    {
        private const int IMAGE_CONTENT_TYPE_DEFINITION = 25;
        private const string IMAGE_TYPE_PNG = "image/png";


        private readonly IGirafService _giraf;

        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        /// <summary>
        /// Constructor for UserController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="roleManager">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
        public UserController(
            IGirafService giraf,
            ILoggerFactory loggerFactory,
            RoleManager<GirafRole> roleManager, 
            IAuthenticationService authentication)
        {
            if (giraf == null) {
                throw new System.ArgumentNullException(giraf + " is null");
            } else if (loggerFactory == null) {
                throw new System.ArgumentNullException(loggerFactory + " is null");
            }
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("User");
            _roleManager = roleManager;
            _authentication = authentication;
        }


        /// <summary>
        /// Find information about the currently authenticated user.
        /// </summary>
        /// <returns> If success returns Meta-data about the currently authorized user else UserNotFound /</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserInfo()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            return Ok(new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true))));
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>  Data about the user if success else MissingProperties, UserNotFound or NotAuthorized </returns>
        [HttpGet("{id}", Name="GetUserById")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserInfo(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("User id not found")));

            //First attempt to fetch the user and check that he exists
            var user =  _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            return Ok(new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true))));
        }

        /// <summary>
        /// Get user-settings for the user with the specified Id
        /// </summary>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get settings for </param>
        /// <returns> UserSettings for the user if success else MissingProperties, UserNotFound, NotAuthorized or RoleMustBeCitizien</returns>
        [HttpGet("{id}/settings", Name="GetUserSettings")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSettings(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("User id not found")));

            //First attempt to fetch the user and check that he exists
            var user = _giraf._context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            // Get the role the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);
            
            //Returns the user settings if the user is a citizen otherwise returns an error (only citizens has settings). 
            if (userRole == GirafRoles.Citizen)
                return Ok(new SuccessResponse<SettingDTO>(new SettingDTO(user.Settings)));
            else
                return BadRequest(new ErrorResponse(ErrorCode.RoleMustBeCitizien, new string("User role must be a citizen")));
        }

        /// <summary>
        /// Updates the user with the information in <see cref="GirafUserDTO"/>
        /// </summary>
        /// <param name="id">identifier of the <see cref="GirafUser"/> to be updated</param>
        /// <param name="newUser">ref to <see cref="GirafUserDTO"/></param>
        /// <returns>DTO for the updated user on success else MissingProperties, UserNotFound, NotAuthorized,
        /// or UserAlreadyExists</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] GirafUserDTO newUser)
        {
            if (newUser == null || newUser.Username == null || newUser.DisplayName == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing user, userName or displayName")));

            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));
            
            // check whether user with that username already exist that does not have the same id
            if (_giraf._context.Users.Any(u => u.UserName == newUser.Username && u.Id != user.Id))
                return Conflict(new ErrorResponse(ErrorCode.UserAlreadyExists, new string("Username already exists")));


            // update fields if they are not null
            if (!String.IsNullOrEmpty(newUser.Username))
                await _giraf._userManager.SetUserNameAsync(user, newUser.Username).ConfigureAwait(true);

            if (!String.IsNullOrEmpty(newUser.DisplayName))
                user.DisplayName = newUser.DisplayName;

            // save and return 
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);
            return Ok(new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, userRole)));
        }

        #region UserIcon
        /// <summary>
        /// Endpoint for getting the UserIcon for a specific User
        /// </summary>
        /// <returns>The requested image as a <see cref="ImageDTO"/></returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/>to get UserIcon for</param>
        [HttpGet("{id}/icon", Name="GetUserIcon")]
        [ProducesResponseType(typeof(SuccessResponse<ImageDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult> GetUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Task.FromResult<ActionResult>(
                    NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found"))));
                

            if (user.UserIcon == null)
                return Task.FromResult<ActionResult>(
                    NotFound(new ErrorResponse(ErrorCode.UserHasNoIcon, new string("User has no icon"))));

            return Task.FromResult<ActionResult>(Ok(new SuccessResponse<ImageDTO>(new ImageDTO(user.UserIcon))));
        }

        /// <summary>
        /// Gets the raw user icon for a given user
        /// </summary>
        /// <returns>The user icon as a png</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get icon for</param>
        [HttpGet("{id}/icon/raw")]
        [Produces(IMAGE_TYPE_PNG)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult> GetRawUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return Task.FromResult<ActionResult>(NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found"))));

            if (user.UserIcon == null)
                return Task.FromResult<ActionResult>(NotFound(new ErrorResponse(ErrorCode.UserHasNoIcon, new string("User has no icon"))));


            return Task.FromResult<ActionResult>(File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(user.UserIcon)), IMAGE_TYPE_PNG));
        }

        /// <summary>
        /// Sets the user icon of the given user
        /// </summary>
        /// <returns>The success response on success else UserNotFound, NotAuthorized, or MissingProperties.</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to set icon for</param>
        [HttpPut("{id}/icon")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SetUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));
            

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body).ConfigureAwait(true);

            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Image is corrupt")));

            user.UserIcon = image;
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            return Ok(new SuccessResponse("User icon set"));
        }

        /// <summary>
        /// Deletes the user icon for a given user
        /// </summary>
        /// <returns>Success response on success else UserHasNoIcon or NotAuthorized </returns>
        /// <param name="id">Identifier.</param>
        [HttpDelete("{id}/icon")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user.UserIcon == null)
                return NotFound(new ErrorResponse(ErrorCode.UserHasNoIcon, new string("User has no icon")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            user.UserIcon = null;
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            return Ok(new SuccessResponse("Icon deleted"));
        }
        #endregion

        #region Methods for adding relations to entities for user
        /// <summary>
        /// Add a ressource to another user that the currently authorised user already owns
        /// </summary>
        /// <returns>The user the resource was added to if success else MissingProperties, UserNotFound, NotAuthorized,
        /// ResourceNotfound, ResourceMustBePrivate, UserAlreadyOwnsResource</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to add the ressource to</param>
        /// <param name="resourceIdDTO">reference to a  <see cref="ResourceIdDTO"/></param>
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        [HttpPost("{id}/resource")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddUserResource(string id, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(id))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing username")));

            if (resourceIdDTO == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing resourceIdDTO")));

            //Attempt to find the target user and check that he exists
            var user = _giraf._context.Users.Include(u => u.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.Pictograms
                .Where(pf => pf.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync().ConfigureAwait(true);

            if (resource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, new string("Resource not found")));

            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return BadRequest(new ErrorResponse(ErrorCode.ResourceMustBePrivate, new string("Resource must be private")));


            //Check that the currently authenticated user owns the resource
            var curUsr = await _giraf.LoadBasicUserDataAsync(HttpContext.User).ConfigureAwait(true);
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, curUsr).ConfigureAwait(true);
            if (!resourceOwnedByCaller)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not own resource")));

            //Check if the target user already owns the resource
            if (user.Resources.Any(ur => ur.PictogramKey == resourceIdDTO.Id))
                return BadRequest(new ErrorResponse(ErrorCode.UserAlreadyOwnsResource, new string("User already owns resource")));

            //Create the relation and save changes.
            var userResource = new UserResource(user, resource);
            await _giraf._context.UserResources.AddAsync(userResource);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            // Get the roles the user is associated with
            GirafRoles userRole = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);

            return Ok(new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, userRole)));
        }

        /// <summary>
        /// Deletes the resource of the user with the given Id
        /// </summary>
        /// <returns>The User the resource was added to on success else UserNotFound, ResourceNotFound, NotAuthorized,
        /// or UserDoesNotOwnResource</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to delete the resource for</param>
        /// <param name="resourceIdDTO">Reference to <see cref="ResourceIdDTO"/></param>
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        [HttpDelete("{id}/resource")]
        [ProducesResponseType(typeof(SuccessResponse<GirafUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteResource(string id, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if the caller owns the resource
            var user = _giraf._context.Users
                .Include(r => r.Resources)
                .ThenInclude(dr => dr.Pictogram)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));
            
            //Check that valid parameters have been specified in the call
            if (resourceIdDTO == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing resourceIdDTO")));

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync().ConfigureAwait(true);
            if (resource == null) return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, new string("Resource not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            //Fetch the relationship from the database and check that it exists
            var relationship = await _giraf._context.UserResources
                 .Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == user.Id)
                .FirstOrDefaultAsync().ConfigureAwait(true);
            if (relationship == null) 
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.UserDoesNotOwnResource, new string("Resource is not owned by user")));

            //Remove the resource - both from the user's list and the database
            user.Resources.Remove(relationship);
            _giraf._context.UserResources.Remove(relationship);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);

            //Return Ok and the user - the resource is now visible in user.Resources
            return Ok(new SuccessResponse<GirafUserDTO>(new GirafUserDTO(user, userRole)));
        }

        /// <summary>
        /// Gets the citizens of the user with the provided id. The provided user must be a guardian
        /// </summary>
        /// <returns>List of <see cref="DisplayNameDTO"/> on success else MissingProperties, NotAuthorized, Forbidden,
        /// or UserNasNoCitizens</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get citizens for</param>
        [HttpGet("{id}/citizens", Name="GetCitizensOfUser")]
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<List<DisplayNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCitizens(string id)
        {
            if (String.IsNullOrEmpty(id))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing id")));
            var user = _giraf._context.Users.Include(u => u.Citizens).FirstOrDefault(u => u.Id == id);
            var authUser = await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true);
            var citizens = new List<DisplayNameDTO>();

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(authUser, user).ConfigureAwait(true)))
            {
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));
            }

            var userRole = (await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true));
            if (userRole != GirafRoles.Guardian)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.Forbidden, new string("User does not have permission")));

            foreach (var citizen in user.Citizens)
            {
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == citizen.CitizenId);
                citizens.Add(new DisplayNameDTO { UserId = girafUser.Id, DisplayName = girafUser.DisplayName });
            }

            if (!citizens.Any())
            {
                return NotFound(new ErrorResponse(ErrorCode.UserHasNoCitizens, new string("User does not have any citizens")));
            }   

            return Ok(new SuccessResponse<List<DisplayNameDTO>>(citizens.ToList<DisplayNameDTO>()));
        }

        /// <summary>
        /// Gets the guardians for the specific citizen corresponding to the provided id.
        /// </summary>
        /// <returns>List of Guardians on success else InvalidProperties, NotAuthorized, Forbidden,
        /// or UserHasNoGuardians </returns>
        /// <param name="id">Identifier for the citizen to get guardians for</param>
        [HttpGet("{id}/guardians", Name="GetGuardiansOfUser")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<List<DisplayNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetGuardians(string id)
        {
            var user = _giraf._context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, new string("Missing id")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            var userRole = (await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true));
            if (userRole != GirafRoles.Citizen)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.Forbidden, new string("User does not have permission")));

            var guardians = new List<DisplayNameDTO>();
            foreach (var guardian in user.Guardians)
            {
                var girafUser = _giraf._context.Users.FirstOrDefault(u => u.Id == guardian.GuardianId);
                guardians.Add(new DisplayNameDTO { UserId = girafUser.Id, DisplayName = girafUser.DisplayName });
            }

            if (!guardians.Any())
            {
                return NotFound(new ErrorResponse(ErrorCode.UserHasNoGuardians, new string("User has no guardians")));
            }

            return Ok(new SuccessResponse<List<DisplayNameDTO>>(guardians));
        }

        /// <summary>
        /// Adds relation between the authenticated user (guardian) and an existing citizen.
        /// </summary>
        /// <param name="id">Guardian id</param>
        /// <param name="citizenId">Citizen id</param>
        /// <returns>Success Reponse on Success else UserNotFound, NotAuthorized, UserNotFound, MissingProperties,
        /// or forbidden </returns>
        [HttpPost("{id}/citizens/{citizenId}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<List<DisplayNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddGuardianCitizenRelationship(string id, string citizenId)
        {
            var citizen = _giraf._context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == citizenId);
            var guardian = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (guardian == null || citizen == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), guardian).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));
                

            var citRole = _roleManager.findUserRole(_giraf._userManager, citizen).Result;
            var guaRole = _roleManager.findUserRole(_giraf._userManager, guardian).Result;

            if (citRole != GirafRoles.Citizen || guaRole != GirafRoles.Guardian)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.Forbidden, new string("User does not have permission")));

            citizen.AddGuardian(guardian);

            return Ok(new SuccessResponse("Added relation between guardian and citizen"));
        }

        /// <summary>
        /// Updates the user settings for the user with the provided id
        /// </summary>
        /// <returns>The updated user settings as a <see cref="SettingDTO"/> on success else UserNotFound,
        /// MissingSettings, NotAuthorized, MissingProperties, InvalidProperties, ColorMustHaveUniqueDay, 
        /// IvalidDay, InvalidHexValues or RoleMustBeCitizien </returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to update settings for</param>
        /// <param name="options">reference to a <see cref="SettingDTO"/> containing the new settings</param>
        [HttpPut("{id}/settings")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<SettingDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateUserSettings(string id, [FromBody] SettingDTO options)
        {
            var user = _giraf._context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            var userRole = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);
            if (userRole != GirafRoles.Citizen)
                return BadRequest(new ErrorResponse(ErrorCode.RoleMustBeCitizien, new string("User role is not citizen")));

            if (user.Settings == null)
                return NotFound(new ErrorResponse(ErrorCode.MissingSettings, new string("User settings not found")));
            
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true), user).ConfigureAwait(true)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            if (!ModelState.IsValid)
                return BadRequest(new ErrorResponse(
                    ErrorCode.MissingProperties, 
                    new string("Missing properties in model"), 
                    "Errors: " + String.Join(", ",ModelState.Values.Where(E => E.Errors.Count > 0)
                        .SelectMany(e => e.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray())));
            
            if (options == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing settings")));

            var error = ValidateOptions(options);
            if (error.HasValue)
                return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, new string("Invalid settings")));

            if(options.WeekDayColors != null) {
                // Validate Correct format of WeekDayColorDTOs. A color must be set for each day
                if (options.WeekDayColors.GroupBy(d => d.Day).Any(g => g.Count() != 1))
                    return BadRequest(new ErrorResponse(ErrorCode.ColorMustHaveUniqueDay, new string("Colors are not set")));


                // check if all days in weekdaycolours is valid
                if (options.WeekDayColors.Any(w => !Enum.IsDefined(typeof(Day), w.Day)))
                    return BadRequest(new ErrorResponse(ErrorCode.InvalidDay, new string("Invalid day")));

                // check that Colors are in correct format
                var isCorrectHexValues = IsWeekDayColorsCorrectHexFormat(options);
                if (!isCorrectHexValues)
                    return BadRequest(new ErrorResponse(ErrorCode.InvalidHexValues, new string("Invalid hex values")));
            }

            user.Settings.UpdateFrom(options);
            // lets update the weekday colours

            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            return Ok(new SuccessResponse<SettingDTO>(new SettingDTO(user.Settings)));
        }

        #endregion
        #region Helpers

        /// <summary>
        /// Check that enum values for settings is defined
        /// </summary>
        /// <returns>ErrorCode if any settings is invalid else null</returns>
        /// <param name="options">ref to <see cref="SettingDTO"/></param>
        private static ErrorCode? ValidateOptions(SettingDTO options)
        {
            if (!(Enum.IsDefined(typeof(Orientation), options.Orientation)) ||
                !(Enum.IsDefined(typeof(CompleteMark), options.CompleteMark)) ||
                !(Enum.IsDefined(typeof(CancelMark), options.CancelMark)) ||
                !(Enum.IsDefined(typeof(DefaultTimer), options.DefaultTimer)) ||
                !(Enum.IsDefined(typeof(Theme), options.Theme))) 
            {
                return ErrorCode.InvalidProperties;
            }
            if (options.NrOfDaysToDisplay < 1 || options.NrOfDaysToDisplay > 7)
            {
                return ErrorCode.InvalidProperties;
            }
            if (options.ActivitiesCount.HasValue && options.ActivitiesCount.Value < 1) 
            {
                return ErrorCode.InvalidProperties;
            }

            if (options.TimerSeconds.HasValue && options.TimerSeconds.Value < 1)
            {
                return ErrorCode.InvalidProperties;
            }

            return null;
        }

        /// <summary>
        /// // Takes a list of WeekDayColorDTOs and check if all hex given is in correct format
        /// </summary>
        private static bool IsWeekDayColorsCorrectHexFormat(SettingDTO setting){
            var regex = new Regex(@"#[0-9a-fA-F]{6}");
            foreach (var weekDayColor in setting.WeekDayColors)
            {
                Match match = regex.Match(weekDayColor.HexColor);
                if (!match.Success)
                    return false;
            }
            return true;
        }


        #endregion
    }
}
