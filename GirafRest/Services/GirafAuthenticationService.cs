using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.Services
{

    /// <summary>
    /// A service used by the EmailService. Contains all the necessary information for the EmailService.
    /// </summary>
    public class GirafAuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Database Context for authentication
        /// </summary>
        public GirafDbContext context { get; }

        /// <summary>
        /// Role manager for authentication roles
        /// </summary>
        public RoleManager<GirafRole> roleManager { get; }

        /// <summary>
        /// User Manager for Users
        /// </summary>
        public UserManager<GirafUser> userManager { get; }

        /// <summary>
        /// Constructor for GirafAuthenticationService
        /// </summary>
        /// <param name="context">DBContext</param>
        /// <param name="roleManager">Role Manager to be used</param>
        /// <param name="userManager">User Manager to be used</param>
        public GirafAuthenticationService(GirafDbContext context, RoleManager<GirafRole> roleManager, UserManager<GirafUser> userManager)
        {
            this.context = context;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// Given the authenticated user and the id on another user this methods check if the authenticated user
        /// has the access to edit the provided user's userinformation.
        /// Does not currently support parents.
        /// </summary>
        public async Task<bool> HasEditOrReadUserAccess(GirafUser authUser, GirafUser userToEdit)
        {
            if (authUser == null || userToEdit == null)
                return false;
            
            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);
            var userRole =  await roleManager.findUserRole(userManager, userToEdit).ConfigureAwait(true);

            if (authUser.Id == userToEdit.Id)
                return true;

            if (userRole == Role.SuperUser)
                return false;

            switch (authUserRole)
            {
                case Role.Citizen:
                    return false;
                case Role.Guardian:
                    if (authUser.DepartmentKey != userToEdit.DepartmentKey || userRole != Role.Citizen)
                        return false;
                    break;
                case Role.Department:
                    if (authUser.DepartmentKey != userToEdit.DepartmentKey)
                        return false;
                    break;
                case Role.SuperUser:
                    break;
                default:
                    break;
            }

            return true;
        }

        /// <summary>
        /// Checks that a user gots the rights to register a specific role to a specific department
        /// Citizen can never register, department and guardian can only register guardians and citizens in same dep
        /// Super user can register all roles
        /// </summary>
        public async Task<bool> HasRegisterUserAccess(GirafUser authUser, Role roleToAdd, long departmentKey)
        {
            if (authUser == null)
                return false;

            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);

            if (authUserRole == Role.Citizen)
                return false;

            if (authUserRole == Role.Guardian || authUserRole == Role.Department)
            {
                if (!(roleToAdd == Role.Guardian || roleToAdd == Role.Citizen) 
                    && departmentKey == authUser.DepartmentKey)
                {
                    return false;
                }
            }
            // only super users can add Department role in fact a super user can do anything so just return true
            return true;
        }

        /// <summary>
        /// Check that a user has access to read the template of a given user
        /// </summary>
        public async Task<bool> HasTemplateAccess(GirafUser authUser)
        {
            if (authUser == null)
                return false;

            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);
            if (authUserRole == Role.Citizen)
                return false;
            return true;
        }

        /// <summary>
        /// Method for checking if a user has access to a departments template
        /// </summary>
        /// <param name="authUser">User to check</param>
        /// <param name="departmentKey">Key of department</param>
        /// <returns>Does the user have access.</returns>
        public async Task<bool> HasTemplateAccess(GirafUser authUser, long? departmentKey)
        {
            if (departmentKey == null)
                return false;

            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);
            if (authUserRole == Role.SuperUser)
                return true;

            return HasTemplateAccess(authUser).Result && authUser?.DepartmentKey == departmentKey;
        }

        /// <summary>
        /// Method for checking whether the authenticated user is allowed to view information related to given department.
        /// </summary>
        public async Task<bool> HasReadDepartmentAccess(GirafUser authUser, long? departmentKey)
        {
            if (authUser == null) 
                throw new System.ArgumentNullException(nameof(authUser) + new string(" is null"));
            
            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);
            if (authUserRole == Role.SuperUser)
                return true;

            return authUser.DepartmentKey == departmentKey;
        }

        /// <summary>
        /// Method for checking if user has access to edit a department
        /// </summary>
        /// <param name="authUser">User to check</param>
        /// <param name="departmentKey">Key of department</param>
        /// <returns></returns>
        public async Task<bool> HasEditDepartmentAccess(GirafUser authUser, long? departmentKey)
        {
            var authUserRole = await roleManager.findUserRole(userManager, authUser).ConfigureAwait(true);
            return authUserRole == Role.SuperUser;
        }
    }
}