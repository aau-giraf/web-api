using GirafRest.Models;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using GirafRest.Models.Responses;
using GirafRest.Extensions;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The RoleController is responsible for managing the roles of the system, and should be used to add or remove various users from roles
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class RoleController : Controller
    {
        /// <summary>
        /// Reference to the girafservice which contains helper method shared by most controllers
        /// </summary>
        private readonly IGirafService _giraf;
        /// <summary>
        /// Reference to the RoleManager, which is necessary to use when manipulating roles in asp.net
        /// </summary>
        private readonly RoleManager<GirafRole> _roleManager;

        /// <summary>
        /// Constructor for the Role-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf"> A reference to the girafservce </param>
        /// <param name="roleManager">A reference to the ASP.NET RoleManager with type GirafRole.</param>
        public RoleController(IGirafService giraf, RoleManager<GirafRole> roleManager)
        {
            _giraf = giraf;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Gets the role of a given user
        /// </summary>
        /// <returns> Returns Ok with the role of the user.</returns>
        [HttpGet("")]
        [Authorize]
        public async Task<Response<string>> GetGirafRole()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            
            if (user == null)
                return new Response<string>("");
            
            var role = await _giraf._userManager.GetRolesAsync(user);
            
            if (role.Count == 0)
                return new Response<string>("");

            return new Response<string>(role[0]);
        }

        /// <summary>
        /// Adds a specified user to the Guardian role
        /// </summary>
        /// <param name="username">Username of the user who needs be to made guardian</param>
        /// <returns>
        ///  
        /// </returns>
        [HttpPost("guardian/{username}")]
        public async Task<Response> AddToGuardian(string username)
        {
            if (GetGirafRole().Result.Data != GirafRole.Department) return new ErrorResponse(ErrorCode.NotAuthorized);

            return await addUserToRoleAsync(username, GirafRole.Guardian);
        }

        /// <summary>
        /// Removes the user from the Guardian role
        /// </summary>
        /// <param name="username">The username of the user in question</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        [HttpDelete("guardian/{username}")]
        public async Task<Response> RemoveFromGuardian(string username)
        {
            if (GetGirafRole().Result.Data != GirafRole.Department) return new ErrorResponse(ErrorCode.NotAuthorized);

            return await removeUserFromRoleAsync(username, GirafRole.Guardian);
        }

        /// <summary>
        /// Adds the user to the Admin role
        /// </summary>
        /// <param name="username">The username of the user in question</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        [HttpPost("admin/{username}")]
        public async Task<Response> AddToAdmin(string username)
        {
            if (GetGirafRole().Result.Data != GirafRole.SuperUser) return new ErrorResponse(ErrorCode.NotAuthorized);

            return await addUserToRoleAsync(username, GirafRole.SuperUser);
        }

        /// <summary>
        /// Removes the user from the Admin role
        /// </summary>
        /// <param name="username">The username of the user in question</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        //[HttpDelete("guardian/{username}")]
        //[Authorize(Policy = GirafRole.RequireSuperUser)]
        //public async Task<IActionResult> RemoveFromAdmin(string username)
        //{
        //    return await removeUserFromRoleAsync(username, GirafRole.SuperUser);
        //}

        /// <summary>
        /// Adds the user to the Department role.
        /// </summary>
        /// <param name="username">The username of the user in question</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        //[HttpPost("admin/{username}")]
        //[Authorize(Policy = GirafRole.RequireSuperUser)]
        //public async Task<IActionResult> AddToDepartment(string username)
        //{
        //    return await addUserToRoleAsync(username, GirafRole.Department);
        //}

        /// <summary>
        /// Removes the user from the Admin role
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        //[HttpDelete("guardian/{username}")]
        //[Authorize(Policy = GirafRole.RequireSuperUser)]
        //public async Task<IActionResult> RemoveFromDepartment(string username)
        //{
        //    return await removeUserFromRoleAsync(username, GirafRole.SuperUser);
        //}½
        
        /// <summary>
        /// Removes a specified role
        /// </summary>
        /// <param name="id">The Id of the role in need of removal
        /// <returns> Badrequest if the role does not exist or if the id was null/emtpy and Ok if successful</returns>
        [HttpDelete("{id}")]
        public async Task<Response> DeleteGirafRole(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                GirafRole girafRole = await _roleManager.FindByIdAsync(id);
                if (girafRole != null)
                {
                    var res = await _roleManager.DeleteAsync(girafRole);
                    if (res.Succeeded) 
                        return new Response();
                }
            }
            return new ErrorResponse(ErrorCode.RoleNotFound);
        }
        #region Helpers

        /// <summary>
        /// Adds a specified user to the specified role.
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <param name="rolename">The name of the role
        /// <returns> NotFound if no such user exists, Badrequest if the operation fails. And Ok if all is well</returns>
        private async Task<Response> addUserToRoleAsync(string username, string rolename)
        {
            var user = await _giraf._userManager.FindByNameAsync(username);
            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            var result = await _giraf._userManager.AddToRoleAsync(user, rolename);
            if (result.Succeeded)
                return new Response();
            else
                return new ErrorResponse(ErrorCode.RoleNotFound);
        }

        /// <summary>
        /// Removes a specified user to the specified role.
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <param name="rolename">The name of the role
        /// <returns> NotFound if no such user exists, Badrequest if the operation fails. And Ok if all is well</returns>
        private async Task<Response> removeUserFromRoleAsync(string username, string rolename)
        {
            var user = await _giraf._userManager.FindByNameAsync(username);
            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            var result = await _giraf._userManager.RemoveFromRoleAsync(user, rolename);
            if (result.Succeeded)
                return new ErrorResponse(ErrorCode.UserNotFound);
            else
                return new ErrorResponse(ErrorCode.RoleNotFound);
        }
        #endregion
    }
}