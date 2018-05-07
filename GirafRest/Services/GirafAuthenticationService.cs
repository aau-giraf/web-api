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
        /// <returns>True if authUser can access userToEdit. False otherwise</returns>
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

        /// <summary>
        /// Checks that a user gots the rights to register a specific role to a specific department
        /// Citizen can never register, department and guardian can only register guardians and citizens in same dep
        /// Super user can register all roles
        /// </summary>
        /// <returns>The to department.</returns>
        /// <param name="authUser">Auth user.</param>
        /// <param name="userToEdit">User to edit.</param>
        public async Task<bool> CheckRegisterRights(GirafUser authUser, GirafRoles roleToAdd, long departmentKey)
        {
            if (authUser == null)
                return false;

            var authUserRole = (await _roleManager.findUserRole(_userManager, authUser));

            if (authUserRole == GirafRoles.Citizen)
                return false;

            if (authUserRole == GirafRoles.Guardian || authUserRole == GirafRoles.Department)
            {
                if (!(roleToAdd == GirafRoles.Guardian || roleToAdd == GirafRoles.Citizen) 
                    && departmentKey == authUser.DepartmentKey)
                {
                    return false;
                }
            }
            // only super users can add Department role in fact a super user can do anything so just return true
            return true;
        }
    }
}