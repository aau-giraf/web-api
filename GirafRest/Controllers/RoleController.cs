using GirafRest.Models;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class RoleController : Controller
    {
        private readonly RoleManager<GirafRole> _roleManager;
        private readonly IGirafService _giraf;

        public RoleController(RoleManager<GirafRole> roleManager)
        {
            _roleManager = roleManager;
        }

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

        [HttpPost("guardian/{username}")]
        [Authorize(Policy = GirafRole.RequireGuardianOrAdmin)]
        public async Task<IActionResult> AddToGuardian(string username)
        {
            return await addUserToRoleAsync(username, GirafRole.Guardian);
        }
        [HttpDelete("guardian/{username]")]
        [Authorize(Policy = GirafRole.RequireGuardianOrAdmin)]
        public async Task<IActionResult> RemoveFromGuardian(string username)
        {
            return await removeUserFromRoleAsync(username, GirafRole.Guardian);
        }

        [HttpPost("admin/{username}")]
        [Authorize(Policy = GirafRole.RequireAdmin)]
        public async Task<IActionResult> AddToAdmin(string username)
        {
            return await addUserToRoleAsync(username, GirafRole.Admin);
        }
        [HttpDelete("guardian/{username]")]
        [Authorize(Policy = GirafRole.RequireGuardianOrAdmin)]
        public async Task<IActionResult> RemoveFromAdmin(string username)
        {
            return await removeUserFromRoleAsync(username, GirafRole.Admin);
        }
        
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
