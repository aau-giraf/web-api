using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.User.DTOs;

namespace GirafServices.User
{
    /// <summary>
    /// The IGirafService interfaces defines all methods that are commonly used by the controllers.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// A reference to the user manager, used to fetch users.
        /// </summary>
        UserManager<GirafUser> _userManager
        {
            get;
            set;
        }

        /// <summary>
        /// Loads only the user with the given username, excluding any associated data.
        /// </summary>
        /// <param name="principal">A reference to HttpContext.User</param>
        /// <returns>The loaded user.</returns>
        Task<GirafUser> LoadBasicUserDataAsync(ClaimsPrincipal principal);

        List<DisplayNameDTO> FindMembers(IEnumerable<GirafUser> users, RoleManager<GirafRole> roleManager, IUserService girafService);

        /// <summary>
        /// Loads the user with resources.
        /// </summary>
        /// <returns>The user with resources.</returns>
        /// <param name="id">The security claim - i.e. the information about the currently authenticated user's ID.</param>
        Task<GirafUser> LoadUserWithResources(ClaimsPrincipal id);

        /// <summary>
        /// Checks if the current user owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="user">A reference to the user in question.</param>
        /// <returns>True if the user owns the resource, false if not.</returns>
        Task<bool> CheckPrivateOwnership(Pictogram resource, GirafUser user);

        /// <summary>
        /// Checks if the current user's department owns the given resource.
        /// </summary>
        /// <param name="resource">The resource to check ownership for.</param>
        /// <param name="user">A reference to the user in question.</param>
        /// <returns>True if the user's department owns the resource, false if not.</returns>
        Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user);

        /// <summary>
        /// Loads the user with department.
        /// </summary>
        /// <returns>The user with department.</returns>
        /// <param name="principal">Principal.</param>
        Task<GirafUser> LoadUserWithDepartment(ClaimsPrincipal principal);

        public Task<GirafUser> LoadUserWithWeekSchedules(string id);

        /// <summary>
        /// Add guardians to registered user
        /// </summary>
        /// <param name="user">The registered user</param>
        void AddGuardiansToUser(GirafUser user);

        /// <summary>
        /// Add citizens to registered user
        /// </summary>
        /// <param name="user">The registered user</param>
        void AddCitizensToUser(GirafUser user);

        void AddCitizen(GirafUser citizen, GirafUser guardian);
        void AddGuardian(GirafUser guardian, GirafUser user);



    }
}
