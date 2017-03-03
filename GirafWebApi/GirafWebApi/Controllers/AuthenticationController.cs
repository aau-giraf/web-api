using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
<<<<<<< Updated upstream

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

        // GET api/users/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            try {
                return Ok(_context.Users.Where((GirafUser u) => u.Id == id));
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

        // PUT api/users/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
=======
        public readonly UserManager<GirafUser> _userManager;
        public readonly SignInManager<GirafUser> _signInManager;
>>>>>>> Stashed changes
    }
}
