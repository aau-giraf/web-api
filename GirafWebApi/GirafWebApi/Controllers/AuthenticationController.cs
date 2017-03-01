using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Data;
using GirafWebApi.Models;

namespace GirafWebApi.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        public readonly GirafDbContext _context;

        // GET api/users
        [HttpGet]
        public IEnumerable<GirafUser> Get()
        {
            return _context.Users.ToList();
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public GirafUser Get(string id)
        {
            return _context.Users.Where((GirafUser u) => u.Id == id).First();
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
    }
}
