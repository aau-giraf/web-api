using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Setup;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Data;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    //[Authorize(Policy = "Admin")]
    public class DepartmentController : Controller
    {
        GirafDbContext _context;
        public DepartmentController(GirafDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
        /// .Include is used to get information on members aswell when getting the Department
            try {
                var depart = _context.Departments.Include(dep => dep.Members);
                return Ok(await depart.ToListAsync()); //Adding Department to context?
            } catch (Exception e) {
                return NotFound("No departments found. " + e.Message);
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long ID)
        {
        /// .Include is used to get information on members aswell when getting the Department
            var department = await _context.Departments.Include(dep => dep.Members).Where(dep => dep.Key == ID).FirstAsync();
            try {
                return Ok(department); 
            } catch (Exception) {
                return NotFound("Department not found. ");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Department dep)
        {
            try
            {
                await _context.Departments.AddAsync(dep);
                _context.SaveChanges();
                return Ok(dep.Name);
            }
            catch (System.Exception e)
            {
                return BadRequest (e.Message + e.InnerException);
            }
        }
    }
}