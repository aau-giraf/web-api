using System.Text.RegularExpressions;
using GirafEntities.Responses;
using GirafEntities.Settings.DTOs;
using GirafEntities.User;
using GirafEntities.User.DTOs;
using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using GirafRepositories;
using GirafRepositories.User;

namespace GirafServices.User
{
    /// <summary>
    /// The GirafService class implements the <see cref="IUserService"/> interface and thus implements common
    /// functionality that is needed by most controllers.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IGirafUserRepository _girafUserRepository;
        private readonly IUserResourseRepository _userResourseRepository;
        private readonly IGirafRoleRepository _girafRoleRepository;
        private readonly IDepartmentResourseRepository _departmentResourseRepository;


        /// <summary>
        /// Asp.net's user manager. Can be used to fetch user data from the request's cookie. Handled by asp.net's dependency injection.
        /// </summary>
        public UserManager<GirafUser> _userManager { get; set; }
        /// <summary>
        /// </summary>

        /// <summary>
        /// The most general constructor for GirafService. This constructor is used by both the other constructors and the unit tests.
        /// </summary>
        /// <param name="userManager">Reference to asp.net's user-manager.</param>
        /// <param name="girafUserRepository">Service Injection</param>
        /// <param name="userResourseRepository">Service Injection</param>
        /// <param name="departmentResourseRepository">Service Injection</param>
        public UserService(UserManager<GirafUser> userManager,
            IGirafUserRepository girafUserRepository,
            IUserResourseRepository userResourseRepository,
            IDepartmentResourseRepository departmentResourseRepository, IGirafRoleRepository girafRoleRepository)
        {
            _userManager = userManager;
            _girafUserRepository = girafUserRepository;
            _userResourseRepository = userResourseRepository;
            _departmentResourseRepository = departmentResourseRepository;
            _girafRoleRepository = girafRoleRepository;
        }
        /// <summary>
        /// Find belonging members
        /// </summary>
        /// <returns>List of matching users</returns>
        public virtual List<DisplayNameDTO> FindMembers(IEnumerable<GirafUser> users, RoleManager<GirafRole> roleManager, IUserService girafService)
        {
            return new List<DisplayNameDTO>(
                users.Select(m => new DisplayNameDTO(
                        m.DisplayName,
                        roleManager.findUserRole(girafService._userManager, m).Result,
                        m.Id
                    )
                ));
        }

        /// <summary>
        /// Method for loading user from context and eager loading <b>resources</b> fields
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithResources(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _girafUserRepository.LoadUserWithResources(usr);
        }

        /// <summary>
        /// Method for loading user from context and eager loading <b>resources</b> fields
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithDepartment(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _girafUserRepository.LoadUserWithDepartment(usr);
        }

        /// <summary>
        /// Method for loading user from context and eager loading fields requied to read their <b>week schedules</b>
        /// </summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithWeekSchedules(string id)
        {
            var user = await _girafUserRepository.LoadUserWithWeekSchedules(id);

            return user;
        }

        /// <summary>
        /// Method for loading user from context, but including no fields. No reference types will be available.
        /// </summary>
        /// <param name="principal">The security claim - i.e. the information about the currently authenticated user.</param>
        /// <returns>A <see cref="GirafUser"/> without any related data.</returns>
        public async Task<GirafUser> LoadBasicUserDataAsync(System.Security.Claims.ClaimsPrincipal principal)
        {
            if (principal == null) return null;
            var usr = (await _userManager.GetUserAsync(principal));
            if (usr == null) return null;
            return await _girafUserRepository.LoadBasicUserDataAsync(usr);
        }

        
        /// <summary>
        /// Checks if the user owns the given <paramref name="pictogram"/>.
        /// </summary>
        /// <param name="pictogram">The pictogram to check the ownership for.</param>
        /// <param name="user"></param>
        /// <returns>True if the user is authorized to see the resource and false if not.</returns>
        public async Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user)
        {
            if (pictogram.AccessLevel != AccessLevel.PRIVATE)
                return false;

            //The pictogram was not public, check if the user owns it.
            if (user == null) return false;
            var ownedByUser = await _userResourseRepository.CheckPrivateOwnership(pictogram, user);

            return ownedByUser;
        }
        
        /// <summary>
        /// Checks if the current user's department owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="user"></param>
        /// <returns>True if the user's department owns the pictogram, false if not.</returns>
        public async Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user)
        {
            if (resource.AccessLevel != AccessLevel.PROTECTED)
                return false;

            if (user == null) return false;

            //The pictogram was not owned by user, check if his department owns it.
            var ownedByDepartment = await _departmentResourseRepository.CheckProtectedOwnership(resource, user);

            return ownedByDepartment;
        }

        /// <summary>
        /// Add guardians to registered user
        /// </summary>
        /// <param name="user">The registered user</param>
        public void AddGuardiansToUser(GirafUser user)
        {
            var guardians = _girafRoleRepository.GetAllGuardians();
            var guardiansInDepartment = _girafUserRepository.GetUsersInDepartment((long)user.DepartmentKey, guardians);
            foreach (var guardian in guardiansInDepartment)
            {
                AddGuardian(guardian, user);
            }
        }

        /// <summary>
        /// Add citizens to registered user
        /// </summary>
        /// <param name="user">The registered user</param>
        public void AddCitizensToUser(GirafUser user)
        {
            var citizens = _girafRoleRepository.GetAllCitizens();
            var citizensInDepartment = _girafUserRepository.GetUsersInDepartment(user.DepartmentKey, citizens);
            foreach (var citizen in citizensInDepartment)
            {
                AddCitizen(citizen, user);
            }
        }

        /// <summary>
        /// Adding citizens 
        /// </summary>
        /// <param name="citizen">Citizen to add</param>
        public void AddCitizen(GirafUser citizen, GirafUser guardian)
        {
            guardian.Citizens.Add(new GuardianRelation(guardian, citizen));
        }

        /// <summary>
        /// Add specific Guardian to this
        /// </summary>
        /// <param name="guardian"></param>
        public void AddGuardian(GirafUser guardian, GirafUser user)
        {
            user.Guardians.Add(new GuardianRelation(guardian, user));
        }

        /// <summary>
        /// Simple helper method for converting a role as enum to a role as string
        /// </summary>
        /// <returns>The role as a string</returns>
        /// <param name="role">A given role as enum that should be converted to a string</param>
        public string GirafRoleFromEnumToString(GirafRoles role)
        {
            switch (role)
            {
                case GirafRoles.Citizen:
                    return GirafRole.Citizen;
                case GirafRoles.Guardian:
                    return GirafRole.Guardian;
                case GirafRoles.Trustee:
                    return GirafRole.Trustee;
                case GirafRoles.Department:
                    return GirafRole.Department;
                case GirafRoles.SuperUser:
                    return GirafRole.SuperUser;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check that enum values for settings is defined
        /// </summary>
        /// <returns>ErrorCode if any settings is invalid else null</returns>
        /// <param name="options">ref to <see cref="SettingDTO"/></param>
        public ErrorCode? ValidateOptions(SettingDTO options)
        {
            if (!(Enum.IsDefined(typeof(Orientation), options.Orientation)) ||
                !(Enum.IsDefined(typeof(CompleteMark), options.CompleteMark)) ||
                !(Enum.IsDefined(typeof(CancelMark), options.CancelMark)) ||
                !(Enum.IsDefined(typeof(DefaultTimer), options.DefaultTimer)) ||
                !(Enum.IsDefined(typeof(Theme), options.Theme)))
            {
                return ErrorCode.InvalidProperties;
            }
            if (options.NrOfDaysToDisplayPortrait < 1 || options.NrOfDaysToDisplayPortrait > 7)
            {
                return ErrorCode.InvalidProperties;
            }
            if (options.NrOfDaysToDisplayLandscape < 1 || options.NrOfDaysToDisplayLandscape > 7)
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
        public bool IsWeekDayColorsCorrectHexFormat(SettingDTO setting)
        {
            var regex = new Regex(@"#[0-9a-fA-F]{6}");
            foreach (var weekDayColor in setting.WeekDayColors)
            {
                Match match = regex.Match(weekDayColor.HexColor);
                if (!match.Success)
                    return false;
            }
            return true;
        }
    }
}
