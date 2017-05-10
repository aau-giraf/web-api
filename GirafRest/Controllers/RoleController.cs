using GirafRest.Models;
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

        public RoleController(RoleManager<GirafRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /*[HttpGet("{id}")]
        public async Task<IActionResult> GetGirafRole(string id)
        {
            return Ok();
        }

        [HttpPost]
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
