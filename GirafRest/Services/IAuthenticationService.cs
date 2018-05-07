using System;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace GirafRest
{
    /// <summary>
    /// Contains methods for authentication checks
    /// </summary>
    public interface IAuthenticationService
    {
        GirafDbContext _context { get; }

        RoleManager<GirafRole> _roleManager { get; }

        UserManager<GirafUser> _userManager { get; }

        /// <summary>
        /// Checks if a user has access to edit information for another user i.ie that the authentication user
        /// and the userToEdit are the same or that the authorised user is guardian for the user
        /// </summary>
        /// <returns>The user access.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="userToEdit">User to edit.</param>
        Task<bool> CheckUserAccess(GirafUser authUser, GirafUser userToEdit);

        /// <summary>
        /// Method for checking if a specific user has the rights to add a specific role to a given department
        /// </summary>
        /// <returns>The register rights.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="roleToAdd">Role to add.</param>
        /// <param name="departmentKey">Department key.</param>
        Task<bool> CheckRegisterRights(GirafUser authUser, GirafRoles roleToAdd, long departmentKey);
    }
}
