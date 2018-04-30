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
            {
                return ErrorCode.NotAuthorized;
            }

            if (authUserRole == GirafRoles.Guardian)
            {
                if ((authUser.DepartmentKey != userToEdit.DepartmentKey)){
                    return ErrorCode.NotAuthorized;
                }

                if (userRole != GirafRoles.Citizen && userRole != GirafRoles.Guardian){
                    return ErrorCode.NotAuthorized;
                }
            }

            if (authUserRole == GirafRoles.Department)
            {
                if ((authUser.DepartmentKey != userToEdit.DepartmentKey))
                    return ErrorCode.NotAuthorized;
            }

            return null;
        }
    }
}