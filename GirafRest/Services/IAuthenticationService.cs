using System;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;

namespace GirafRest
{
    /// <summary>
    /// Contains methods for authentication checks
    /// </summary>
    public interface IAuthenticationService
    {
        Data.GirafDbContext _context
        {
            get;
        }

        /// <summary>
        /// Checks if a user has access to edit information for another user i.ie that the authentication user
        /// and the userToEdit are the same or that the authorised user is guardian for the user
        /// </summary>
        /// <returns>The user access.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="authUserRole">Auth user role.</param>
        /// <param name="userToEdit">User to edit.</param>
        ErrorCode? CheckUserAccess(GirafUser authUser, GirafRoles authUserRole, GirafUser userToEdit, GirafRoles userRole);
    }
}
