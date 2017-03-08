using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using GirafWebApi.Models;
using IdentityServer4.Configuration;

namespace GirafWebApi.Controllers
{
    //[Route("[controller]")]
    //[Authorize]
    public class IdentityController : ControllerBase
    {
        public readonly UserManager<GirafUser> _userManager;

        public IdentityController(UserManager<GirafUser> uMan) {
            this._userManager = uMan;
        }

        
     /*   
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }*/
    }
}