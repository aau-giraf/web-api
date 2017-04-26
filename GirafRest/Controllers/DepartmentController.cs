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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using GirafRest.Services;

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IGirafService _giraf;

        /// <summary>
        /// Constructor for the department-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="context">A reference to the database-context.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a debug-logger.</param>
        public DepartmentController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
        
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

                if (result.Count == 0)
                    return NotFound();

                //Return the list.
                return Ok(result.Select(d => new DepartmentDTO(d)).ToList());
            } catch (Exception e) {
                _giraf._logger.LogError($"Exception in Get: {e.Message}, {e.InnerException}");
                return BadRequest();
            }
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
            var department = _giraf._context.Departments
                .Where(dep => dep.Key == ID);

            if(!await department.AnyAsync()) return NotFound();
            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstAsync();
            try {
                return Ok(new DepartmentDTO(depa)); 
            } catch (Exception e) {
                _giraf._logger.LogError("Exception in Get{id}: " + e.Message);
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
                Department result = new Department(dep);
                await _giraf._context.Departments.AddAsync(result);

                //Add all members specified by either id or username in the DTO
                foreach(var mem in dep.Members) {
                    var usr = await _giraf._context.Users
                        .Where(u => u.UserName == mem || u.Id == mem)
                        .FirstAsync();

                    if(usr == null) continue;

                    result.Members.Add(usr);
                    usr.Department = result;
                }
                
                //Add all the resources with the given ids
                foreach (var reso in dep.Pictograms) {
                    var res = await _giraf._context.Pictograms
                        .Where(p => p.Id == reso)
                        .FirstAsync();

                    if(res == null) continue;
                    var dr = new DepartmentResource(result, res);
                    await _giraf._context.DepartmentResources.AddAsync(dr);
                }
                //Save the changes and return the entity
                await _giraf._context.SaveChangesAsync();
                return Ok(new DepartmentDTO(result));
            }
            catch (System.Exception e)
            {
                _giraf._logger.LogError($"Exception in Post: {e.Message}, {e.InnerException}");
                return BadRequest (e.Message + e.InnerException);
            }
        }

        /// <summary>
        /// Add a user to the given department.
        /// </summary>
        /// <param name="ID">The Id of the department to add the user to.</param>
        /// <param name="usr">A serialized GirafUser instance to add to the department.</param>
        /// <returns>BadRequest if no user or department has been specified or the user is already in the department,
        ///  NotFound if there is no department with the given ID or
        ///  Ok if there was no problems.</returns>
        [HttpPost("add-user/{id}")]
        public async Task<IActionResult> AddUser(long ID, [FromBody]GirafUser usr)
        {
            //Fetch user and department and check that they exist
            if(usr == null)
                return BadRequest("User was null");
            Department dep;

            try
            {
                dep = await _giraf._context.Departments
                    .Where(d => d.Key == ID)
                    .Include(d => d.Members)
                    .FirstAsync();
            }
            catch (Exception e)
            {
                return NotFound("Department not found");
            }

            //Check if the user is already in the department
            if(dep.Members.Where(u => u.UserName == usr.UserName).Any())
                return BadRequest("User already exists in Department");

            //Add the user and sace the changes
            dep.Members.Add(usr);
            await _giraf._context.SaveChangesAsync();
            return Ok(new DepartmentDTO(dep));
        }

        /// <summary>
        /// Removes a user from a given department.
        /// </summary>
        /// <param name="ID">Id of the department from which the user should be removed</param>
        /// <param name="usr">A serialized instance of a <see cref="GirafUser"/> user.</param>
        /// <returns>
        /// BadRequest if no user is given or he does not exist in the department,
        /// NotFound if there is no department with the given Id or
        /// Ok if no problems occured.
        /// </returns>
        [HttpDelete("remove-user/{id}")]
        public async Task<IActionResult> RemoveUser(long ID, [FromBody]GirafUser usr)
        {
            Department dep = null;

            //Check if a valid user was supplied and that the given department exists
            if(usr == null)
                return BadRequest("User was null");
            try
            {
                dep = await _giraf._context
                    .Departments
                    .Where(d => d.Key == ID)
                    .Include(d => d.Members)
                    .FirstAsync();
            }
            catch (Exception e)
            {
                if(dep == null)
                    return NotFound("Department not found");
            }

            //Check if the user actually is in the department
            if(!dep.Members.Where(u => u.UserName == usr.UserName).Any())
                return BadRequest("User does not exist in Department");

            //Remove the user from the department
            dep.Members.Remove(dep.Members.Where(u => u.UserName == usr.UserName).First());
            _giraf._context.SaveChanges();
            return Ok(new DepartmentDTO(dep));
        }

        /// <summary>
        /// Add a resource to the given department. After this call the department owns the resource and it is available to all its members.
        /// </summary>
        /// <param name="id">Id of the department to add the resource to.</param>
        /// <param name="resourceId">Id of the resource to add.</param>
        /// <returns>
        /// NotFound if either the department or the resource does not exist,
        /// BadRequest if no resourceId has been specified as either query-parameter or in the request-body or
        /// Ok if no problems occured.
        /// </returns>
        [HttpPost("add-resource/{id}")]
        [Authorize]
        public async Task<IActionResult> AddResource(long id, [FromBody] long? resourceId) {
            //Fetch the department and check that it exists.
            var department = await _giraf._context.Departments.Where(d => d.Key == id).FirstAsync();
            if(department == null) return NotFound($"There is no department with Id {id}.");

            //Check if there is a resourceId specified in the body or as a query-paramater
            long resId = -1;
            var resourceIdValid = CheckResourceId(resourceId, ref resId);
            if(!resourceIdValid) return BadRequest("Unable to find a valid resource-id. Please specify one in request-body or as url-query.");

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Frames.Where(f => f.Id == resId).FirstAsync();
            if(resource == null) return NotFound($"There is no resource with id {id}.");

            var resourceOwned = await _giraf.CheckProtectedOwnership(resource, HttpContext);
            if(!resourceOwned) return Unauthorized();

            //Check if the department already owns the resource
            var alreadyOwned = await _giraf._context.DepartmentResources
                .Where(depres => depres.OtherKey == id && depres.ResourceKey == resId)
                .AnyAsync();
            if(alreadyOwned) return BadRequest("The department already owns the given resource.");

            //Create a relationship between the department and the resource.
            var dr = new DepartmentResource(department, resource);
            await _giraf._context.DepartmentResources.AddAsync(dr);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            return Ok(new DepartmentDTO(department));
        }

        [HttpPost("remove-resource/{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveResource(long id, [FromBody] long? resourceId) {
            //Fetch the department and check that it exists.
            var department = await _giraf._context.Departments.Where(d => d.Key == id).FirstAsync();
            if(department == null) return NotFound($"There is no department with Id {id}.");

            long resId = -1;
            var resourceIdValid = CheckResourceId(resourceId, ref resId);
            if(!resourceIdValid) return BadRequest("Unable to find a valid resource-id. Please specify one in request-body or as url-query.");

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Frames
                .Where(f => f.Id == resId)
                .FirstOrDefaultAsync();
            if(resource == null) return NotFound($"There is no resource with id {resourceId}.");

            var resourceOwned = await _giraf.CheckProtectedOwnership(resource, HttpContext);
            if(!resourceOwned) return Unauthorized();

            //Check if the department already owns the resource and remove if so.
            var drrelation = await _giraf._context.DepartmentResources
                .Where(dr => dr.ResourceKey == resource.Id && dr.OtherKey == department.Key)
                .FirstOrDefaultAsync();
            if(drrelation == null) return BadRequest("The department does not own the given resource.");
            department.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            return Ok(new DepartmentDTO(department));
        }

        #region Helpers
        private IQueryable<Department> NameQueryFilter(string nameQuery)
        {
            if(string.IsNullOrEmpty(nameQuery)) nameQuery = "";
            return _giraf._context.Departments.Where(d => d.Name.ToLower().Contains(nameQuery.ToLower()));
        }

        /// <summary>
        /// Checks if a valid resource-id has been specified along with the request.
        /// </summary>
        /// <param name="resourceId">The resource-id specified in the request's body.</param>
        /// <param name="resId">A ref parameter for storing the found resource-id.</param>
        /// <returns>True if a valid resource-id was found, false otherwise.</returns>
        private bool CheckResourceId(long? resourceId, ref long resId) {
            if(resourceId == null) {
                var resourceQuery = HttpContext.Request.Query["resourceId"];
                if(string.IsNullOrEmpty(resourceQuery)) return false;
                if(!long.TryParse(resourceQuery, out resId)) return false;
            }
            else resId = (long) resourceId;
            return true;
        }
        #endregion
    }
}