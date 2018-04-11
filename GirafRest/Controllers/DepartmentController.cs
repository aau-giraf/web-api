using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Identity;
using GirafRest.Extensions;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The department controller serves the purpose of handling departments. It is capable of producing a
    /// list of all departments in the system as well as adding resources and users to departments.
    /// </summary>
    [Route("v1/[controller]")]
    public class DepartmentController : Controller
    {
        /// <summary>
        /// A reference to IGirafService, which defines common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;


        private readonly RoleManager<GirafRole> _roleManager;

        /// <summary>
        /// Constructor for the department-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        public DepartmentController(IGirafService giraf, ILoggerFactory loggerFactory, RoleManager<GirafRole> roleManager)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
            _roleManager = roleManager;
        }

        /// <summary>
        /// Gets the department names.
        /// </summary>
        /// <returns>The department names.</returns>
        [HttpGet("")]
        public async Task<Response<List<DepartmentNameDTO>>> Get()
        {

            var departmentNameDTOs = await _giraf._context.Departments
                                                 .Select(d => new DepartmentNameDTO(d.Key, d.Name)).ToListAsync();

            if (departmentNameDTOs.Count == 0)
                return new ErrorResponse<List<DepartmentNameDTO>>(ErrorCode.NotFound);

            return new Response<List<DepartmentNameDTO>>(departmentNameDTOs);
        }

        /// <summary>
        /// Get the department with the specified id.
        /// </summary>
        /// <param name="id">The id of the department to retrieve.</param>
        /// <returns>NotFound if department with given id does not exsist
        /// DepartmentDTO id the department does exsist.</returns>
        [HttpGet("{id}")]
        public async Task<Response<DepartmentDTO>> Get(long id)
        {
            //.Include is used to get information on members aswell when getting the Department
            var department = _giraf._context.Departments
                .Where(dep => dep.Key == id);

            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstOrDefaultAsync();

            if (depa == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotFound);

            return new Response<DepartmentDTO>(new DepartmentDTO(depa));
        }

        /// <summary>
        /// Gets the citizen names.
        /// </summary>
        /// <returns>The citizen names.</returns>
        /// <param name="id">Department ID.</param>
        [HttpGet("{id}/citizens")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        public async Task<Response<List<UserNameDTO>>> GetCitizenNamesAsync(long id)
        {
            var department = _giraf._context.Departments.FirstOrDefault(dep => dep.Key == id);

            if (department == null) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentNotFound);

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            // eager load department from context
           currentUser =  _giraf._context.Users.Include(a => a.Department)
                                               .FirstOrDefault(d => d.UserName == currentUser.UserName);
                  
            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.Department.Key != department?.Key && !isSuperUser)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);


            var roleCitizenId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Citizen)
                                                     .Select(c => c.Id).FirstOrDefault();

            if (roleCitizenId == null) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentHasNoCitizens);

            var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleCitizenId)
                                .Select(r => r.UserId).Distinct();

            if (!userIds.Any()) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentHasNoCitizens);

            var usersNamesInDepartment = _giraf._context.Users
                                               .Where(u => userIds.Any(ui => ui == u.Id)
                                                      && u.DepartmentKey == department.Key)
                                               .Select(u => new UserNameDTO(u.UserName, u.Id)).ToList();

            return new Response<List<UserNameDTO>>(usersNamesInDepartment);
        }

        /// <summary>
        /// Create a new department. it's only necesary to supply the departments name
        /// </summary>
        /// <param name="depDTO">The department to add to the database.</param>
        /// <returns>The new departmentDTO with all database-generated information.</returns>
        [HttpPost("")]
        [Authorize]
        public async Task<Response<DepartmentDTO>> Post([FromBody]DepartmentDTO depDTO)
        {
            try
            {
                if (depDTO?.Name == null)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties,
                        "Deparment name has to be specified!");

                var authenticatedUser = await _giraf.LoadUserAsync(HttpContext.User);
                
                if(authenticatedUser == null)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
                
                var userRole = await _roleManager.findUserRole(_giraf._userManager, authenticatedUser);

                if (!(await _giraf._userManager.IsInRoleAsync(authenticatedUser, GirafRole.SuperUser)))
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);

                //Add the department to the database.
                Department department = new Department(depDTO);
                
                //Add all members specified by either id or username in the DTO
                if (depDTO.Members != null)
                {
                    foreach (var mem in depDTO.Members)
                    {
                        var usr = await _giraf._context.Users
                            .Where(u => u.UserName == mem.UserName || u.Id == mem.UserId)
                            .FirstOrDefaultAsync();
                        if (usr == null)
                            return new ErrorResponse<DepartmentDTO>(ErrorCode.InvalidProperties,
                                "The member list contained an invalid id: " + mem);
                        department.Members.Add(usr);
                        usr.Department = department;
                    }
                }

                //Add all the resources with the given ids
                if (depDTO.Resources != null)
                {
                    foreach (var reso in depDTO.Resources)
                    {
                        var res = await _giraf._context.Pictograms
                            .Where(p => p.Id == reso)
                            .FirstOrDefaultAsync();
                        if (res == null)
                            return new ErrorResponse<DepartmentDTO>(ErrorCode.InvalidProperties,
                                "The list of resources contained an invalid resource id: " + reso);
                        var dr = new DepartmentResource(department, res);
                        await _giraf._context.DepartmentResources.AddAsync(dr);
                    }
                }

                await _giraf._context.Departments.AddAsync(department);
                
                //Create a new user with the supplied information

                var departmentUser = new GirafUser(depDTO.Name, department) {IsDepartment = true};

                //department.Members.Add(user);
                
                var identityUser = await _giraf._userManager.CreateAsync(departmentUser, "0000");
                
                if (identityUser.Succeeded == false)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.CouldNotCreateDepartmentUser, string.Join("\n", identityUser.Errors));

                await _giraf._userManager.AddToRoleAsync(departmentUser, GirafRole.Department);

                //Save the changes and return the entity
                await _giraf._context.SaveChangesAsync();
                return new Response<DepartmentDTO>(new DepartmentDTO(department));
            }
            catch (System.Exception e)
            {
                var errorDescription = $"Exception in Post: {e.Message}, {e.InnerException}";
                _giraf._logger.LogError(errorDescription);
                return new ErrorResponse<DepartmentDTO>(ErrorCode.Error, errorDescription);
            }
        }

        /// <summary>
        /// Add a user to the given department.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="userId">The ID of a GirafUser to be added to the department.</param>
        /// <returns>MissingProperties if the userId is null
        /// DepartmentNotFound if department of specified ID isn't found
        /// UserNameAlreadyTakenWithinDepartment if a user with usr's username already exists in the specified department
        /// UserNotFound if no user exists with the ID of usr
        /// NotAuthorised if role is guardian/department but you are trying to add a role to a citizen that is not yours
        /// A DepartmentDTO representing the new state of the department, if there were no problems.</returns>
        [HttpPost("{departmentId}/user/{userId}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<DepartmentDTO>> AddUser(long departmentId, string userId)
        {
            //Fetch user and department and check that they exist
            if (userId == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties, "userId");
            
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);
            if(currentUser == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
            
            var role = await _roleManager.findUserRole(_giraf._userManager, currentUser);

            if(role == GirafRoles.Department || role == GirafRoles.Guardian){
                // lets check that we are in the correct department
                if (currentUser.DepartmentKey != departmentId)
                {
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);
                }
            }

            Department dep = await _giraf._context.Departments
                .Where(d => d.Key == departmentId)
                .Include(d => d.Members)
                .FirstOrDefaultAsync();

            if (dep == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            //Check if the user is already in the department
            if (dep.Members.Any(u => u.Id == userId))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNameAlreadyTakenWithinDepartment);

            //Add the user and save these changes
            var user = await _giraf._context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if (user == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);

            user.DepartmentKey = dep.Key;
            dep.Members.Add(user);
            await _giraf._context.SaveChangesAsync();
            return new Response<DepartmentDTO>(new DepartmentDTO(dep));
        }

        /// <summary>
        /// Removes a user from a given department.
        /// </summary>
        /// <param name="departmentId">Id of the department from which the user should be removed</param>
        /// <param name="usr">A serialized instance of a <see cref="GirafUser"/> user.</param>
        /// <returns>
        /// MissingProperties if no user is given.
        /// UserNotFound if user does not appear within given department.
        /// DepartmentNotFound if there is no department with the given Id.
        /// DepartmentDTO in its updated state if no problems occured.
        /// </returns>
        [HttpDelete("user/{departmentID}")]
        public async Task<Response<DepartmentDTO>> RemoveUser(long departmentID, [FromBody]GirafUserDTO usr)
        {
            //Check if a valid user was supplied and that the given department exists
            if (usr == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties);

            var dep = await _giraf._context
                .Departments
                                  .Where(d => d.Key == departmentID)
                .Include(d => d.Members)
                .FirstOrDefaultAsync();

            if (dep == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            //Check if the user actually is in the department
            if (!dep.Members.Any(u => u.UserName == usr.Username))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFoundInDepartment,
                    "User does not exist in the given department.");

            //Remove the user from the department
            dep.Members.Remove(dep.Members.First(u => u.UserName == usr.Username));
            _giraf._context.SaveChanges();
            return new Response<DepartmentDTO>(new DepartmentDTO(dep));
        }

        /// <summary>
        /// Add a resource to the given department. After this call, the department owns the resource and it is available to all its members.
        /// </summary>
        /// <param name="departmentId">Id of the department to add the resource to.</param>
        /// <param name="resourceId">Id of the resource to add to the department.</param>
        /// <returns>
        /// DepartmentNotFound If department wasn't found.
        /// ResourceNotFound If no resource exists with the given ID.
        /// NotAuthorized If user does not have ownership of resource.
        /// DepartmentAlreadyOwnsResource If requested resource is already owned by requested department.
        /// The DepartmentDTO represented the updated state of the department if there were no errors.
        /// </returns>
        [HttpPost("{departmentId}/resource/{resourceId}")]
        [Authorize]
        public async Task<Response<DepartmentDTO>> AddResource(long departmentId, long resourceId)
        {
            //Fetch the department and check that it exists.
            var department = await _giraf._context.Departments.Where(d => d.Key == departmentId).FirstOrDefaultAsync();
            var usr = await _giraf.LoadUserAsync(HttpContext.User);

            if (department == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            //Fetch the resource with the given id, check that it exists and that the user owns it.
            var resource = await _giraf._context.Pictograms.Where(f => f.Id == resourceId).FirstOrDefaultAsync();

            if (resource == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.ResourceNotFound);

            var resourceOwned = await _giraf.CheckPrivateOwnership(resource, usr);

            if (!resourceOwned)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);

            //Check if the department already owns the resource
            var alreadyOwned = await _giraf._context.DepartmentResources
                                           .Where(depres => depres.OtherKey == departmentId 
                                                  && depres.ResourceKey == resourceId)
                .AnyAsync();

            if (alreadyOwned)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentAlreadyOwnsResource);

            //Remove resource from user
            var usrResource = await _giraf._context.UserResources
                .Where(ur => ur.ResourceKey == resource.Id && ur.OtherKey == usr.Id)
                .FirstOrDefaultAsync();
            usr.Resources.Remove(usrResource);
            await _giraf._context.SaveChangesAsync();

            //Change resource AccessLevel to Protected from Private
            resource.AccessLevel = AccessLevel.PROTECTED;

            //Create a relationship between the department and the resource.
            var dr = new DepartmentResource(department, resource);
            await _giraf._context.DepartmentResources.AddAsync(dr);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            return new Response<DepartmentDTO>(new DepartmentDTO(department));
        }

        /// <summary>
        /// Removes a resource from the users department.
        /// </summary>
        /// <param name="resourceId"></param>
        /// <returns>
        /// DepartmentDTO of updated state if no problems occured.
        /// RessourceNotFound if ressource could not be found
        /// NotAuthorised if not authorised to delete ressource
        /// ResourceNotOwnedByDepartment if ressource not owned by department
        /// </returns>
        [HttpDelete("resource/{resourceId}")]
        [Authorize]
        public async Task<Response<DepartmentDTO>> RemoveResource(long resourceId)
        {
            //Fetch the department and check that it exists.
            var usr = await _giraf.LoadUserAsync(HttpContext.User);

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceId)
                .FirstOrDefaultAsync();

            if (resource == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.ResourceNotFound);

            var resourceOwned = await _giraf.CheckProtectedOwnership(resource, usr);

            if (!resourceOwned)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);

            //Check if the department already owns the resource and remove if so.
            var drrelation = await _giraf._context.DepartmentResources
                .Where(dr => dr.ResourceKey == resource.Id && dr.OtherKey == usr.Department.Key)
                .FirstOrDefaultAsync();

            if (drrelation == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.ResourceNotOwnedByDepartment);

            usr.Department.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            return new Response<DepartmentDTO>(new DepartmentDTO(usr.Department));
        }

        #region Helpers
        /// <summary>
        /// Produces a list of all departments that has 'nameQuery' in their name.
        /// </summary>
        /// <param name="nameQuery">A string to search for in the name of the departments.</param>
        /// <returns>A list of all departments with 'nameQuery' in their name.</returns>
        private IQueryable<Department> NameQueryFilter(string nameQuery)
        {
            if (string.IsNullOrEmpty(nameQuery)) nameQuery = "";
            return _giraf._context.Departments.Where(d => d.Name.ToLower().Contains(nameQuery.ToLower()));
        }

        /// <summary>
        /// Checks if a valid resource-id has been specified along with the request.
        /// </summary>
        /// <param name="resourceId">The resource-id specified in the request's body.</param>
        /// <param name="resId">A ref parameter for storing the found resource-id.</param>
        /// <returns>True if a valid resource-id was found, false otherwise.</returns>
        private bool CheckResourceId(long? resourceId, ref long resId)
        {
            if (resourceId == null)
            {
                var resourceQuery = HttpContext.Request.Query["resourceId"];
                if (string.IsNullOrEmpty(resourceQuery)) return false;
                if (!long.TryParse(resourceQuery, out resId)) return false;
            }
            else resId = (long)resourceId;
            return true;
        }
        #endregion
    }
}
