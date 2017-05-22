using GirafRest.Models;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The RoleController is responsible for managing the roles of the system, and should be used to add or remove various users from roles
    /// </summary>
    [Authorize]
    [Route("[controller]")]
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
        /// <param name="id">The Id of the user in question
        /// <returns> Returns Ok with the role of the user.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGirafRole(string id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var role = await _giraf._userManager.GetRolesAsync(user);

            if (role.Count == 0)
                return Ok();

            return Ok(role[0]);
        }

        /// <summary>
        /// Adds a specified user to the Guardian role
        /// </summary>
        /// <param name="username">Username of the user who needs be to made guardian</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation fails. And Ok if all is well</returns>
        [HttpPost("guardian/{username}")]
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        public async Task<IActionResult> AddToGuardian(string username)
        {
            return await addUserToRoleAsync(username, GirafRole.Guardian);
        }

        /// <summary>
        /// Removes the user from the Guardian role
        /// </summary>
        /// <param name="username">The username of the user in question</param>
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        [HttpDelete("guardian/{username}")]
        [Authorize(Policy = GirafRole.RequireGuardianOrSuperUser)]
        public async Task<IActionResult> RemoveFromGuardian(string username)
        {
            return await removeUserFromRoleAsync(username, GirafRole.Guardian);
        }

        /// <summary>
        /// Adds the user from the Admin role
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        [HttpPost("admin/{username}")]
        [Authorize(Policy = GirafRole.RequireSuperUser)]
        public async Task<IActionResult> AddToAdmin(string username)
        {
            return await addUserToRoleAsync(username, GirafRole.SuperUser);
        }

        /// <summary>
        /// Removes the user from the Admin role
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <returns> NotFound if no such user exists, Badrequest if the operation failed and Ok if successful</returns>
        [HttpDelete("guardian/{username}")]
        [Authorize(Policy = GirafRole.RequireSuperUser)]
        public async Task<IActionResult> RemoveFromAdmin(string username)
        {
            return await removeUserFromRoleAsync(username, GirafRole.SuperUser);
        }
        
        /// <summary>
        /// Removes a specified role
        /// </summary>
        /// <param name="id">The Id of the role in need of removal
        /// <returns> Badrequest if the role does not exist or if the id was null/emtpy and Ok if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGirafRole(string id)
        {
            if (!String.IsNullOrEmpty(id))
            {
                GirafRole girafRole = await _roleManager.FindByIdAsync(id);
                if (girafRole != null)
                {
                    var res = await _roleManager.DeleteAsync(girafRole);
                    if (res.Succeeded) return Ok();
                }
                return BadRequest("Role does not exist");
            }
            return BadRequest("Null or empty role id");
        }
        #region Helpers

        /// <summary>
        /// Adds a specified user to the specified role.
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <param name="rolename">The name of the role
        /// <returns> NotFound if no such user exists, Badrequest if the operation fails. And Ok if all is well</returns>
        [Authorize(Policy = GirafRole.SuperUser)]
        private async Task<IActionResult> addUserToRoleAsync(string username, string rolename)
        {
            var user = await _giraf._userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("There is no user with the given id.");

            var result = await _giraf._userManager.AddToRoleAsync(user, rolename);
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Errors);
        }

        /// <summary>
        /// Removes a specified user to the specified role.
        /// </summary>
        /// <param name="username">The username of the user in question
        /// <param name="rolename">The name of the role
        /// <returns> NotFound if no such user exists, Badrequest if the operation fails. And Ok if all is well</returns>
        private async Task<IActionResult> removeUserFromRoleAsync(string username, string rolename)
        {
            var user = await _giraf._userManager.FindByNameAsync(username);
            if (user == null)
                return NotFound("There is no user with the given id.");

            var result = await _giraf._userManager.RemoveFromRoleAsync(user, rolename);
            if (result.Succeeded)
                return Ok();
            else
                return BadRequest(result.Errors);
        }
        #endregion
    }
}
