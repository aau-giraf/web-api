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
        /// reference to the authenticationservice which provides commong authentication checks
        /// </summary>
        private readonly IAuthenticationService _authentication;

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
        /// Find information about the currently authenticated user.
        /// </summary>
        /// <returns>
        /// Meta-data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged -in user is not authorized to see user.
        ///</returns>
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
        /// <returns>
        /// Data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged in user is not authorized to see user.
        ///</returns>
        [HttpGet("{id}")]
        public async Task<Response<GirafUserDTO>> GetUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties, "username");

            //First attempt to fetch the user and check that he exists
            var user =  _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            return new Response<GirafUserDTO>(new GirafUserDTO(user, await _roleManager.findUserRole(_giraf._userManager, user)));
        }

        /// <summary>
        /// Find information on the user with the id given
        /// </summary>
        /// <returns>
        /// Data about the user
        /// MissingProperties if no username is provided
        /// UserNotFound if user was not found, or logged in user is not authorized to see user.
        ///</returns>
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
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<SettingDTO>(ErrorCode.NotAuthorized);

            return new Response<SettingDTO>(new SettingDTO(user.Settings));
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
        [HttpPut("{id}")]
        public async Task<Response<GirafUserDTO>> UpdateUser(string id, [FromBody] string Username, [FromBody] string ScreenName)
        {
            if (Username == null && ScreenName == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.MissingProperties);

            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);
            // Get the roles the user is associated with
            var userRole = await _roleManager.findUserRole(_giraf._userManager, user);
            if (user == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            
            // check whether user with that username already exist that does dot have the same id
            if (_giraf._context.Users.Any(u => u.UserName == Username && u.Id != user.Id))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserAlreadyExists);

            // update fields if they are not null
            if (!String.IsNullOrEmpty(Username))
                user.UserName = Username;

            if (!String.IsNullOrEmpty(ScreenName))
                user.DisplayName = ScreenName;

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
        [HttpPut("{id}/icon")]
        public async Task<Response> SetUserIcon(string id)
        {
            var user = _giraf._context.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);
            

            byte[] image = await _giraf.ReadRequestImage(HttpContext.Request.Body);

            if (image.Length < IMAGE_CONTENT_TYPE_DEFINITION)
                return new ErrorResponse(ErrorCode.MissingProperties, "Image");

            user.UserIcon = image;
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
        [HttpDelete("{id}/icon")]
        public async Task<Response> DeleteUserIcon(string id)
        {
            var user = _giraf._context.Users.Include(u => u.UserIcon).FirstOrDefault(u => u.Id == id);
            if (user.UserIcon == null)
                return new ErrorResponse(ErrorCode.UserHasNoIcon);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

            user.UserIcon = null;
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }
        #endregion
        #region Methods for adding relations to entities for user


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
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
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
            var curUsr = await _giraf.LoadUserAsync(HttpContext.User);
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
        /// Deletes a resource with the specified id from the given user's list of resources.
        /// </summary>
        /// <param name="resourceIdDTO"></param>
        /// <returns>
        /// MissingProperties if resourceIdDTO is null
        /// ResourceNotFound if we cannot find the ressource to delete
        /// UserDoesNotOwnResource if authenticatedUser does not own ressource
        /// The GirafUserDTO if no errors.
        /// </returns>
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
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
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
        /// Gets the citizens for the specific user corresponding to the provided id.
        /// </summary>
        /// <returns>
        /// MissingProperties if username is null
        /// UserHasNoCitizens is user has no citizens
        /// List of all citizens as DTO if ok
        /// </returns>
        /// <param name="username">Username.</param>
        [HttpGet("{id}/citizens")]
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<List<UserNameDTO>>> GetCitizens(string id)
        {
            if (String.IsNullOrEmpty(id))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.MissingProperties, "id");
            var user = await _giraf.LoadByIdAsync(id);
            var citizens = new List<UserNameDTO>();

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);

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
        /// Gets the guardians for the specific user corresponding to the provided id.
        /// </summary>
        /// <returns>
        /// MissingProperties if username is null
        /// UserHasNoGuardians is user has no guardians
        /// List of all guardians if ok </returns>
        /// <param name="id">Username.</param>
        [HttpGet("{id}/guardians")]
        [Authorize]
        public async Task<Response<List<UserNameDTO>>> GetGuardians(string id)
        {
            var user = await _giraf.LoadByIdAsync(id);
            if (user == null)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.InvalidProperties, "id");

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);

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
        /// <param name="id">Username.</param>
        /// <returns>
        /// UserNotFound if a user with the given username does not exsist.
        /// DepartmentNotFound if the user does not belong to any department.
        /// DepartmentDTO in its updated state if no problems occured.
        /// </returns>
        [HttpDelete("{id}/department")]
        public async Task<Response<DepartmentDTO>> RemoveDepartment(string id)
        {
            if(string.IsNullOrEmpty(id))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties, "username");

            var user = await _giraf._context
                .Users
                .Include(u => u.Department)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            
            if(user == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);
            
            var dep = await _giraf._context
                .Departments
                .Where(d => d.Key == user.DepartmentKey)
                .Include(d => d.Members)
                .Include(d => d.Resources)
                .FirstOrDefaultAsync();
            
            if (dep == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            user.DepartmentKey = null;
            _giraf._context.SaveChanges();
            
            return new Response<DepartmentDTO>(new DepartmentDTO(dep));
        }

        [HttpPost("{id}/citizen/{citizenId}")]
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<GirafUserDTO>> AddGuardianCitizenRelationship(string id, string citizenId)
        {
            var citizen = await _giraf._userManager.FindByIdAsync(citizenId);
            var guardian = await _giraf._userManager.FindByIdAsync(id);

            if (guardian == null || citizen == null)
                return new ErrorResponse<GirafUserDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), guardian)))
                return new ErrorResponse<GirafUserDTO>(ErrorCode.NotAuthorized);

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
        /// Updates the user settings for a user with the given id
        /// </summary>
        /// <returns>
        /// MissingProperties if options is null or some required fields is not set
        /// </returns>
        /// <param name="options">Options.</param>
        [HttpPut("{id}/settings")]
        [Authorize]
        public async Task<Response<SettingDTO>> UpdateUserSettings(string id, [FromBody] SettingDTO options)
        {
            if (!ModelState.IsValid)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingProperties, ModelState.Values.Where(E => E.Errors.Count > 0)
                                  .SelectMany(E => E.Errors)
                                  .Select(E => E.ErrorMessage)
                                  .ToArray());
            
            if (options == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingProperties, "Settings");

            var error = ValidateOptions(options);
            if (error.HasValue)
                return new ErrorResponse<SettingDTO>(ErrorCode.InvalidProperties, "Settings");

            // Validate Correct format of WeekDayColorDTOs. A color must be set for each day
            if (options.WeekDayColors.GroupBy(d => d.Day).Any(g => g.Count() != 1))
                return new ErrorResponse<SettingDTO>(ErrorCode.ColorMustHaveUniqueDay);

            // check if all days in weekdaycolours is valid
            if (options.WeekDayColors.Any(w => !Enum.IsDefined(typeof(Days), w.Day)))
                return new ErrorResponse<SettingDTO>(ErrorCode.InvalidDay);

            // check that Colors are in correct format
            var IsCorrectHexValues = IsWeekDayColorsCorrectHexFormat(options);
            if (!IsCorrectHexValues)
                return new ErrorResponse<SettingDTO>(ErrorCode.InvalidHexValues);

            var user = _giraf._context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
            if (user == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.UserNotFound);

            if (user.Settings == null)
                return new ErrorResponse<SettingDTO>(ErrorCode.MissingSettings);
            
            // check access rights
            if (!(await _authentication.CheckUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<SettingDTO>(ErrorCode.NotAuthorized);

            user.Settings.UpdateFrom(options);
            // lets update the weekday colours

            await _giraf._context.SaveChangesAsync();

            return new Response<SettingDTO>(new SettingDTO(user.Settings));
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
                if (!user.Resources.Any(r => r.PictogramKey == resourceId))
                {
                    throw new InvalidOperationException("You may not add pictograms to a user by a PUT request. " +
                        "Please use a POST to user/resource instead");
                }
            }

            //Remove all the resources that are in the user's list, but not in the id-list
            foreach (var resource in user.Resources)
            {
                if (!resourceIds.Contains(resource.PictogramKey))
                    _giraf._context.Remove(resource);
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
        /// Check that enum values for settings is defined
        /// </summary>
        /// <returns>The options.</returns>
        /// <param name="options">Options.</param>
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

        // Takes a list of WeekDayColorDTOs and check if the hex given is in correct format
        private bool IsWeekDayColorsCorrectHexFormat(SettingDTO setting){
            var regex = new Regex(@"#[0-9a-fA-F]{6}");
            foreach (var weekDayColor in setting.WeekDayColors)
            {
                var hexColor = weekDayColor.HexColor;
                Match match = regex.Match(weekDayColor.HexColor);
                if (!match.Success)
                    return false;
            }
            return true;
        }


        #endregion
    }
}
