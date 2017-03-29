using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Setup;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Data;
using Microsoft.Extensions.Logging;
using GirafRest.Models.DTOs;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class DepartmentController : Controller
    {
        /// <summary>
        /// A reference to the database-context - used for quering data.
        /// </summary>
        private readonly GirafDbContext _context;
        /// <summary>
        /// A reference to a logger, used to provide debug information.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor for the department-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="context">A reference to the database-context.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a debug-logger.</param>
        public DepartmentController(GirafDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger<DepartmentController>();
        }

        /// <summary>
        /// Get all departments registered in the database.
        /// </summary>
        /// <returns>A list of all departments.</returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try {
                //Filter all departments away that does not satisfy the name query.
                var nameQuery = HttpContext.Request.Query["name"];
                var depart = NameQueryFilter(nameQuery);
                //Include relevant information and cast to list. .Select does not seem to work on IEnumerables.
                var result = await depart
                    .Include(dep => dep.Members)
                    .Include(dep => dep.Resources)
                    .ThenInclude(dr => dr.Resource)
                    .ToListAsync();

                //Return the list.
                return Ok(result.Select(d => new DepartmentDTO(d)).ToList());
            } catch (Exception e) {
                _logger.LogError($"Exception in Get: {e.Message}, {e.InnerException}");
                return BadRequest();
            }
        }

        private IQueryable<Department> NameQueryFilter(string nameQuery)
        {
            if(string.IsNullOrEmpty(nameQuery)) nameQuery = "";
            return _context.Departments.Where(d => d.Name.ToLower().Contains(nameQuery.ToLower()));
        }

        /// <summary>
        /// Get the department with the specified id.
        /// </summary>
        /// <param name="ID">The id of the department to search for.</param>
        /// <returns>The department with the given id or NotFound.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long ID)
        {
        /// .Include is used to get information on members aswell when getting the Department
            var department = _context.Departments
                .Where(dep => dep.Key == ID);

            if(!await department.AnyAsync()) return NotFound();
            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstAsync();
            try {
                return Ok(new DepartmentDTO(depa)); 
            } catch (Exception e) {
                _logger.LogError("Exception in Get{id}: " + e.Message);
                return NotFound("Department not found.");
            }
        }

        /// <summary>
        /// Add a department to the database.
        /// </summary>
        /// <param name="dep">The department to add to the database.</param>
        /// <returns>The new department with all database-generated information.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody]DepartmentDTO dep)
        {
            try
            {
                //Add the department to the database.
                var result = await _context.Departments.AddAsync(new Department(dep));

                //Add all members specified by either id or username in the DTO
                foreach(var mem in dep.Members) {
                    var usr = await _context.Users
                        .Where(u => u.UserName == mem || u.Id == mem)
                        .FirstAsync();

                    if(usr == null) continue;

                    result.Entity.Members.Add(usr);
                    usr.Department = result.Entity;
                }
                //Add all the resources with the given ids
                foreach (var reso in dep.Pictograms) {
                    var res = await _context.Pictograms
                        .Where(p => p.Key == reso)
                        .FirstAsync();

                    if(res == null) continue;
                    var dr = new DepartmentResource(result.Entity, res);
                    await _context.DepartmentResources.AddAsync(dr);
                }

                //Save the changes and return the entity
                await _context.SaveChangesAsync();
                return Ok(new DepartmentDTO(result.Entity));
            }
            catch (System.Exception e)
            {
                _logger.LogError($"Exception in Post: {e.Message}, {e.InnerException}");
                return BadRequest (e.Message + e.InnerException);
            }
        }

        [HttpPost("{id}/add-user")]
        public async Task<IActionResult> AddUser(long ID, [FromBody]GirafUser usr)
        {
            if(usr == null)
                return BadRequest("User was null");
            var dep = await _context.Departments.Include(d => d.Members).Where(d => d.Key == ID).FirstAsync();
            if(dep == null)
                return NotFound("Department not found");
            if(dep.Members.Where(u => u.UserName == usr.UserName).Any())
                return BadRequest("User already exists in Department");
            dep.Members.Add(usr);
            _context.SaveChanges();
            return Ok("User added succesfully");
        }
        [HttpDelete("{id}/remove-user")]
        public async Task<IActionResult> RemoveUser(long ID, [FromBody]GirafUser usr)
        {
            if(usr == null)
                return BadRequest("User was null");
            var dep = await _context.Departments.Include(d => d.Members).Where(d => d.Key == ID).FirstAsync();
            if(dep == null)
                return NotFound("Department not found");
            if(!dep.Members.Where(u => u.UserName == usr.UserName).Any())
                return BadRequest("User does not exist in Department");
            dep.Members.Remove(dep.Members.Where(u => u.UserName == usr.UserName).First());
            _context.SaveChanges();
            return Ok("User removed succesfully");
        }
    }
}