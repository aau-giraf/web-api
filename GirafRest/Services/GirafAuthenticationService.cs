using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;

namespace GirafRest.Services
{

    /// <summary>
    /// A service used by the EmailService. Contains all the necessary information for the EmailService.
    /// </summary>
    public class GirafAuthenticationService : IAuthenticationService
    {
        public GirafDbContext _context { get; }

        public GirafAuthenticationService(GirafDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        ///  Given the authenticated user and the id on another user this methods check if the authenticated user
        /// has the access to edit the provided users userinformation
        /// </summary>
        /// <returns>null if no errorcodes</returns>
        public ErrorCode? CheckUserAccess(GirafUser authUser, GirafRoles authUserRole, GirafUser userToEdit, GirafRoles userRole)
        {
            if (authUserRole == GirafRoles.Citizen && authUser.Id != userToEdit.Id)
                return ErrorCode.NotAuthorized;

            if (userRole != GirafRoles.Citizen  && authUser.Id != userToEdit.Id && authUserRole != GirafRoles.SuperUser)
                return ErrorCode.NotAuthorized;

            if (authUserRole == GirafRoles.Guardian || authUserRole == GirafRoles.Department)
            {
                if ((authUser.DepartmentKey != userToEdit.DepartmentKey) && (authUser.DepartmentKey != null || authUser.DepartmentKey != -1))
                    return ErrorCode.NotAuthorized;
                // check that dep key is not set and check if the user are in there current list of guardians
                else if (authUserRole == GirafRoles.Guardian)
                {
                    var childOfGuardian = _context.Users.Any(u => u.Id == authUser.Id
                                                            && u.Citizens.Any(c => c.CitizenId == userToEdit.Id));
                    if (!childOfGuardian)
                        return ErrorCode.NotAuthorized;
                }
            }
            return null;
        }
    }
}