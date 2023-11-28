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
        /// A data-logger used to write messages to the console. Handled by asp.net's dependency injection.
        /// </summary>
        public ILogger _logger { get; set; }

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
            Console.WriteLine("Calling get all guardians");
            var guardians = _girafRoleRepository.GetAllGuardians();
            Console.WriteLine("Go go go go go");
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

    }
}
