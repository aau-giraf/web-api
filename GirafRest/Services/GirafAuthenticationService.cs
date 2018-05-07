using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.Services
{

    /// <summary>
    /// A service used by the EmailService. Contains all the necessary information for the EmailService.
    /// </summary>
    public class GirafAuthenticationService : IAuthenticationService
    {
        public GirafDbContext _context { get; }

        public RoleManager<GirafRole> _roleManager { get; }

        public UserManager<GirafUser> _userManager { get; }

        public GirafAuthenticationService(GirafDbContext context, RoleManager<GirafRole> roleManager, UserManager<GirafUser> userManager)
        {
            this._context = context;
            this._roleManager = roleManager;
            this._userManager = userManager;
        }

        /// <summary>
        ///  Given the authenticated user and the id on another user this methods check if the authenticated user
        /// has the access to edit the provided users userinformation
        /// Does not currently support parents
        /// </summary>
        /// <returns>null if no errorcodes</returns>
        public async Task<bool> CheckUserAccess(GirafUser authUser, GirafUser userToEdit)
        {
            if (authUser == null || userToEdit == null)
                return false;
            
            var authUserRole = (await _roleManager.findUserRole(_userManager, authUser));
            var userRole =  (await _roleManager.findUserRole(_userManager, userToEdit));

            if (authUser.Id == userToEdit.Id)
                return true;

            if (authUserRole == GirafRoles.Citizen && authUser.Id != userToEdit.Id)
                return false;

            if (authUserRole == GirafRoles.Guardian)
            {
                if ((authUser.DepartmentKey != userToEdit.DepartmentKey))
                    return false;

                if (userRole != GirafRoles.Citizen && userRole != GirafRoles.Guardian)
                    return false;
            }

            if (authUserRole == GirafRoles.Department)
            {
                if ((authUser.DepartmentKey != userToEdit.DepartmentKey))
                    return false;
            }

            return true;
        }
    }
}