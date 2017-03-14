using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        GirafDbContext _context;
        public DepartmentController(GirafDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {
        /// .Include is used to get information on members aswell when getting the Department
            try {
                var depart = _context.Departments.Include(x => x.members);
                return Ok(depart.ToList()); 
            } catch (Exception e) {
                return NotFound("No departments found. " + e.Message);
            }
        }
        [HttpGet("{id}")]
        public IActionResult Get(long ID)
        {
        /// .Include is used to get information on members aswell when getting the Department
            var department = _context.Departments.Include(x => x.members).Where(dep => dep.Key == ID).First();
            try {
                return Ok(department); 
            } catch (Exception) {
                return NotFound("Department not found. ");
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Department dep)
        {
            try
            {
                _context.Departments.Add(dep);
                _context.SaveChanges();
                return Ok(dep.Name);
            }
            catch (System.Exception e)
            {
                return Ok(e.Message + e.InnerException);
            }
        }
    }
}