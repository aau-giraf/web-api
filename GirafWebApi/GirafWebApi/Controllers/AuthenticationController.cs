using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
    public class AuthenticationController : Controller
    {
        public readonly GirafDbContext _context;
        public readonly UserManager<GirafUser> _userManager;
        public readonly SignInManager<GirafUser> _signInManager;

        public AuthenticationController(GirafDbContext context, UserManager<GirafUser> um, SignInManager<GirafUser> sim)
        {
            this._context = context;
            this._userManager = um;
            this._signInManager = sim;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try {
                return Ok(await _context.Users.ToListAsync());
            } catch (Exception) {
                return NotFound("No users found");
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
        public async Task<IActionResult> Post([FromBody]GirafUser user)
        {
            
        }

        // PUT api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
        }
    }
}
