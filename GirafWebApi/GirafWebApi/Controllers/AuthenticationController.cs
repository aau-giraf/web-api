using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.AspNetCore.Identity;

namespace GirafWebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        public readonly GirafDbContext _context;

        // GET api/users
        [HttpGet]
        public IActionResult Get()
        {
            try {
                return Ok(_context.Users.ToList());
            } catch (Exception) {
                return NotFound("No user found");
            }
        }

        // POST api/users
        [HttpPost]
        public IActionResult Post([FromBody]GirafUser user)
        {
            // TODO: add password to the search criteria for a GirafUser
            var db_user = _context.Users.Where((GirafUser u) => u.UserName.Equals(user.UserName)).First();

            if(db_user == null)
            {
                return NotFound();
            }
            return Ok();
        }
        public readonly UserManager<GirafUser> _userManager;
        public readonly SignInManager<GirafUser> _signInManager;
    }
}
