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

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("v1/[controller]")]
    public class UserController : Controller
    {
        private const int IMAGE_CONTENT_TYPE_DEFINITION = 25;

        private readonly IGirafService _giraf;

        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        public UserController(
            IGirafService giraf,
          ILoggerFactory loggerFactory,
          RoleManager<GirafRole> roleManager, 
            IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("User");
            _roleManager = roleManager;
            _authentication = authentication;
        }


        /// <summary>
        /// Find information about the currently authenticated user.
        /// </summary>
        /// <param name="id">Identifier of a <see cref="GirafUser"/></param>
        /// <returns> If success returns Meta-data about the currently authorized user else UserNotFound /</returns>
        [HttpGet("")]
        public async Task<Response<GirafUserDTO>> GetUser()
        {
            //First attempt to fetch the user and check that he exists
            var user = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, await _roleManager.findUserRole(_giraf._userManager, user)));
        }

        /// <summary>
        /// Find information on the user with the username supplied as a url query parameter or the current user.
        /// </summary>
        /// <returns>  Data about the user if success else MissingProperties, UserNotFound or NotAuthorized </returns>
        [HttpGet("{id}")]
        public async Task<Response<GirafUserDTO>> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "id");

            //First attempt to fetch the user and check that he exists
            var user =  _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, await _roleManager.findUserRole(_giraf._userManager, user)));
        }

        /// <summary>
        /// Get user-settings for the user with the specified Id
        /// </summary>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get settings for </param>
        /// <returns> UserSettings for the user if success else MissingProperties, UserNotFound, NotAuthorized or RoleMustBeCitizien</returns>
        [HttpGet("{id}/settings")]
        public async Task<Response<SettingDTO>> GetSettings(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingProperties, "id");

            //First attempt to fetch the user and check that he exists
            var user = _giraf._context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<SettingDTO>(ErrorCode.NotAuthorized);

            // Get the role the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);
            
            //Returns the user settings if the user is a citizen otherwise returns an error (only citizens has settings). 
            if (userRole == GirafRoles.Citizen)
                return new Response<SettingDTO>(new SettingDTO(user.Settings));
            else
                return new ErrorResponse<SettingDTO>(ErrorCode.RoleMustBeCitizien);
        }

        /// <summary>
        /// Updates the user with the information in <see cref="GirafUserDTO"/>
        /// </summary>
        /// <param name="id">identifier of the <see cref="GirafUser"/> to be updated</param>
        /// <param name="newUser">ref to <see cref="GirafUserDTO"/></param>
        /// <returns>DTO for the updated user on success else MissingProperties, UserNotFound, NotAuthorized,
        /// or UserAlreadyExists</returns>
        [HttpPut("{id}")]
        public async Task<Response<GirafUserDTO>> UpdateUser(string id, [FromBody] GirafUserDTO newUser)
        {
            if (newUser == null || newUser.Username == null || newUser.ScreenName == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);

            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            
            // check whether user with that username already exist that does dot have the same id
            if (_giraf._context.Users.Any(u => u.UserName == newUser.Username && u.Id != user.Id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyExists);


            // update fields if they are not null
            if (!String.IsNullOrEmpty(newUser.Username))
                await _giraf._userManager.SetUserNameAsync(user, newUser.Username);

            if (!String.IsNullOrEmpty(newUser.ScreenName))
                user.DisplayName = newUser.ScreenName;

            // save and return 
            _giraf._context.Users.Update(user);
            await _giraf._context.SaveChangesAsync();
            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        #region UserIcon
        /// <summary>
        /// Endpoint for getting the UserIcon for a specific User
        /// </summary>
        /// <returns>The requested image as a <see cref="ImageDTO"/></returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/>to get UserIcon for</param>
        [HttpGet("{id}/icon")]
        public Task<Response<ImageDTO>> GetUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return Task.FromResult<Response<ImageDTO>>(new ErrorResponse<ImageDTO>(ErrorCode.UserNotFound));
                

            if (user.UserIcon == null)
                return Task.FromResult<Response<ImageDTO>>(new ErrorResponse<ImageDTO>(ErrorCode.UserHasNoIcon));

            return Task.FromResult(new Response<ImageDTO>(new ImageDTO(user.UserIcon)));
        }

        /// <summary>
        /// Gets the raw user icon for a given user
        /// </summary>
        /// <returns>The user icon as a png</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get icon for</param>
        [HttpGet("{id}/icon/raw")]
        public Task<IActionResult> GetRawUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);

            if (user == null)
                return Task.FromResult<IActionResult>(NotFound());

            if (user.UserIcon == null)
                return Task.FromResult<IActionResult>(NotFound());


            return Task.FromResult<IActionResult>(File(Convert.FromBase64String(System.Text.Encoding.UTF8.GetString(user.UserIcon)), "image/png"));
        }

        /// <summary>
        /// Sets the user icon of the given user
        /// </summary>
        /// <returns>The success response on success else UserNotFound, NotAuthorized, or MissingProperties.</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to set icon for</param>
        [HttpPut("{id}/icon")]
        public async Task<Response> SetUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);

            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION)
                return new ErrorResponse(ErrorCode.MissingProperties, "Image");

            user.UserIcon = image;
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }

        /// <summary>
        /// Deletes the user icon for a given user
        /// </summary>
        /// <returns>Success response on success else UserHasNoIcon or NotAuthorized </returns>
        /// <param name="id">Identifier.</param>
        [HttpDelete("{id}/icon")]
        public async Task<Response> DeleteUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);
            if (user.UserIcon == null)
                return new ErrorResponse(ErrorCode.UserHasNoIcon);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            user.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return new Response();
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
        public async Task<Response<GirafUserDTO>> AddUserResource(string id, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if valid parameters have been specified in the call
            if (string.IsNullOrEmpty(id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");

            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "resourceIdDTO");

            //Attempt to find the target user and check that he exists
            var user = _giraf._context.Users.Include(u => u.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);

            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            //Find the resource and check that it actually does exist - also verify that the resource is private
            var resource = await _giraf._context.Pictograms
                .Where(pf => pf.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();

            if (resource == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceNotFound);

            if (resource.AccessLevel != AccessLevel.PRIVATE)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceMustBePrivate);


            //Check that the currently authenticated user owns the resource
            var curUsr = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            var resourceOwnedByCaller = await _giraf.CheckPrivateOwnership(resource, curUsr);
            if (!resourceOwnedByCaller)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            //Check if the target user already owns the resource
            if (user.Resources.Any(ur => ur.PictogramKey == resourceIdDTO.Id))
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
        /// Deletes the resource of the user with the given Id
        /// </summary>
        /// <returns>The User the resource was added to on success else UserNotFound, ResourceNotFound, NotAuthorized,
        /// or UserDoesNotOwnResource</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to delete the resource for</param>
        /// <param name="resourceIdDTO">Reference to <see cref="ResourceIdDTO"/></param>
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        [HttpDelete("{id}/resource")]
        public async Task<Response<GirafUserDTO>> DeleteResource(string id, [FromBody] ResourceIdDTO resourceIdDTO)
        {
            //Check if the caller owns the resource
            var user = _giraf._context.Users.Include(r => r.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);
            
            //Check that valid parameters have been specified in the call
            if (resourceIdDTO == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "resourceIdDTO");

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceIdDTO.Id)
                .FirstOrDefaultAsync();
            if (resource == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.ResourceNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            //Fetch the relationship from the database and check that it exists
            var relationship = await _giraf._context.UserResources
                 .Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == user.Id)
                .FirstOrDefaultAsync();
            if (relationship == null) return new ErrorResponse<GirafUserDTO>(ErrorCode.UserDoesNotOwnResource);

            //Remove the resource - both from the user's list and the database
            user.Resources.Remove(relationship);
            _giraf._context.UserResources.Remove(relationship);
            await _giraf._context.SaveChangesAsync();

            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);

            //Return Ok and the user - the resource is now visible in user.Resources
            return new Response<GirafUserDTO>(new GirafUserDTO(user, userRole));
        }

        /// <summary>
        /// Gets the citizens of the user with the provided id. The provided user must be a guardian
        /// </summary>
        /// <returns>List of <see cref="UserNameDTO"/> on success else MissingProperties, NotAuthorized, Forbidden,
        /// or UserNasNoCitizens</returns>
        /// <param name="id">Identifier of the <see cref="GirafUser"/> to get citizens for</param>
        [HttpGet("{id}/citizens")]
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<List<UserNameDTO>>> GetCitizens(string id)
        {
            if (String.IsNullOrEmpty(id))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.MissingProperties, "id");
            var user = _giraf._context.Users.Include(u => u.Citizens).FirstOrDefault(u => u.Id == id);
            var authUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            var citizens = new List<UserNameDTO>();

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(authUser, user)))
            {
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);
            }

            var userRole = (await _roleManager.findUserRole(_giraf._userManager, user));
            if (userRole != GirafRoles.Guardian)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.Forbidden);;

            foreach (var citizen in user.Citizens)
            {
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
        /// Gets the guardians for the specific citizen corresponding to the provided id.
        /// </summary>
        /// <returns>List of Guardians on success else InvalidProperties, NotAuthorized, Forbidden,
        /// or UserHasNoGuardians </returns>
        /// <param name="id">Identifier for the citizen to get guardians for</param>
        [HttpGet("{id}/guardians")]
        [Authorize]
        public async Task<Response<List<UserNameDTO>>> GetGuardians(string id)
        {
            var user = _giraf._context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.InvalidProperties, "id");

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);

            var userRole = (await _roleManager.findUserRole(_giraf._userManager, user));
            if (userRole != GirafRoles.Citizen)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.Forbidden); ;

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
        /// Adds relation between the authenticated user (guardian) and an existing citizen.
        /// </summary>
        /// <param name="id">Guardian id</param>
        /// <param name="citizenId">Citizen id</param>
        /// <returns>Success Reponse on Success else UserNotFound, NotAuthorized, UserNotFound, MissingProperties,
        /// or forbidden </returns>
        [HttpPost("{id}/citizens/{citizenId}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response> AddGuardianCitizenRelationship(string id, string citizenId)
        {
            var citizen = _giraf._context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == citizenId);
            var guardian = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (guardian == null || citizen == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), guardian)))
                return new ErrorResponse(ErrorCode.NotAuthorized);

            var citRole = _roleManager.findUserRole(_giraf._userManager, citizen).Result;
            var guaRole = _roleManager.findUserRole(_giraf._userManager, guardian).Result;

            if (citRole != GirafRoles.Citizen || guaRole != GirafRoles.Guardian)
                return new ErrorResponse(ErrorCode.Forbidden);

            citizen.AddGuardian(guardian);

            return new Response();
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
        public async Task<Response<SettingDTO>> UpdateUserSettings(string id, [FromBody] SettingDTO options)
        {
            var user = _giraf._context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.UserNotFound);

            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);
            if (userRole != GirafRoles.Citizen)
                return new ErrorResponse<SettingDTO>(ErrorCode.RoleMustBeCitizien);

            if (user.Settings == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingSettings);
            
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<SettingDTO>(ErrorCode.NotAuthorized);

            if (!ModelState.IsValid)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(e => e.Errors)
                                  .Select(e => e.ErrorMessage)
                                  .ToArray());
            
            if (options == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingProperties, "Settings");

            var error = ValidateOptions(options);
            if (error.HasValue)
                return new ErrorResponse<SettingDTO>(ErrorCode.InvalidProperties, "Settings");

            if(options.WeekDayColors != null) {
                // Validate Correct format of WeekDayColorDTOs. A color must be set for each day
                if (options.WeekDayColors.GroupBy(d => d.Day).Any(g => g.Count() != 1))
                    return new ErrorResponse<SettingDTO>(ErrorCode.ColorMustHaveUniqueDay);


                // check if all days in weekdaycolours is valid
                if (options.WeekDayColors.Any(w => !Enum.IsDefined(typeof(Days), w.Day)))
                    return new ErrorResponse<SettingDTO>(ErrorCode.InvalidDay);

                // check that Colors are in correct format
                var isCorrectHexValues = IsWeekDayColorsCorrectHexFormat(options);
                if (!isCorrectHexValues)
                    return new ErrorResponse<SettingDTO>(ErrorCode.InvalidHexValues);
            }

            user.Settings.UpdateFrom(options);
            // lets update the weekday colours

            await _giraf._context.SaveChangesAsync();

            return new Response<SettingDTO>(new SettingDTO(user.Settings));
        }

        #endregion
        #region Helpers

        /// <summary>
        /// Check that enum values for settings is defined
        /// </summary>
        /// <returns>ErrorCode if any settings is invalid else null</returns>
        /// <param name="options">ref to <see cref="SettingDTO"/></param>
        private ErrorCode? ValidateOptions(SettingDTO options)
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
            if (options.CompletedActivityOption < 1 || options.CompletedActivityOption > 3)
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
        private bool IsWeekDayColorsCorrectHexFormat(SettingDTO setting){
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
