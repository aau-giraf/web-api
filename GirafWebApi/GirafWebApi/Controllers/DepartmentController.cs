using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafWebApi.Contexts;
using GirafWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GirafWebApi.Controllers
{
    [Route("[controller]")]
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
            try {
                var depart = _context.Departments.Include(x => x.members);
                return Ok(depart.ToList()); //Adding Department to context?
            } catch (Exception e) {
                return NotFound("No departments found. " + e.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(long ID)
        {
            var department = _context.Departments.Include(x => x.members).Where(dep => dep.Key == ID).First();
            try {
                return Ok(department); //Adding Department to context?
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