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

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetGirafRole(string id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var role = await _giraf._userManager.GetRolesAsync(user);

            if (role.Count == 0)
                return Ok();

            return Ok(role[0]);
        }

        /*[HttpPost]
        public async Task<IActionResult> CreateGirafRole()
        {
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateGirafRole()
        {
            return Ok();
        }
        */
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
    }
}
