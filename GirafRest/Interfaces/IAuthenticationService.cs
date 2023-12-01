using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.Enums;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace GirafRest.Interfaces
{
    /// <summary>
    /// Contains methods for authentication checks
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Database context
        /// </summary>
        GirafDbContext _context { get; }

        /// <summary>
        /// Role Manager to be used
        /// </summary>
        RoleManager<GirafRole> _roleManager { get; }

        /// <summary>
        /// User manager to be used.
        /// </summary>
        UserManager<GirafUser> _userManager { get; }

        /// <summary>
        /// Checks if a user has access to edit information for another user i.ie that the authentication user
        /// and the userToEdit are the same or that the authorised user is guardian for the user
        /// </summary>
        /// <returns>The user access.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="userToEdit">User to edit.</param>
        Task<bool> HasEditOrReadUserAccess(GirafUser authUser, GirafUser userToEdit);

        /// <summary>
        /// Method for checking if a specific user has the rights to add a specific role to a given department
        /// </summary>
        /// <returns>The register rights.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="roleToAdd">Role to add.</param>
        /// <param name="departmentKey">Department key.</param>
        Task<bool> HasRegisterUserAccess(GirafUser authUser, GirafRoles roleToAdd, long departmentKey);

        /// <summary>
        /// Method for checking wheteher the authenticated user is allowed to edit and view templates in general. 
        /// </summary>
        /// <param name="authUser">The user in question</param>
        Task<bool> HasTemplateAccess(GirafUser authUser);

        /// <summary>
        /// Method for checking wheteher the authenticated user is allowed to edit and view templates in the given department. 
        /// </summary>
        /// <param name="authUser">The user in question</param>
        /// <param name="departmentKey">Department of the template in question</param>
        Task<bool> HasTemplateAccess(GirafUser authUser, long? departmentKey);

        /// <summary>
        /// Method for checking whether the authenticated user is allowed to view information related to given department.
        /// </summary>
        /// <param name="authUser">The user in question</param>
        /// <param name="departmentKey">Department in question</param>
        /// <returns></returns>
        Task<bool> HasReadDepartmentAccess(GirafUser authUser, long? departmentKey);

        /// <summary>
        /// Method for checking whether the authenticated user is allowed to view information related to given department.
        /// </summary>
        /// <param name="authUser">The user in question</param>
        /// <param name="departmentKey">Department in question</param>
        /// <returns></returns>
        Task<bool> HasEditDepartmentAccess(GirafUser authUser, long? departmentKey);
    }
}
