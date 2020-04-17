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
using System;

namespace GirafRest.Controllers
{
    [Route("v1/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IGirafService _giraf;

        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        public DepartmentController(IGirafService giraf, 
            ILoggerFactory loggerFactory, 
            RoleManager<GirafRole> roleManager, 
            IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
            _roleManager = roleManager;
            _authentication = authentication;
        }

        /// <summary>
        /// Get request for getting all the department names.
        /// </summary>
        /// <returns>A list of department names, returns NotFound if no departments in the system</returns>
        [HttpGet("")]
        public async Task<Response<List<DepartmentNameDTO>>> Get()
        {

            var departmentNameDTOs = await _giraf._context.Departments.Select(d => new DepartmentNameDTO(d.Key, d.Name)).ToListAsync();

            if (departmentNameDTOs.Count == 0)
                return new ErrorResponse<List<DepartmentNameDTO>>(ErrorCode.NotFound);

            return new Response<List<DepartmentNameDTO>>(departmentNameDTOs);
        }

        /// <summary>
        /// Get the department with the specified id.
        /// </summary>
        /// <param name="id">The id of the <see cref="Department"/> to retrieve.</param>
        /// <returns>The department as a DepartmentDTO if success else UserNotfound, NotAuthorised or NotFound</returns>
        [HttpGet("{id}")]
        public async Task<Response<DepartmentDTO>> Get(long id)
        {
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
                  
            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != id && !isSuperUser)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotAuthorized);
            
            //.Include is used to get information on members aswell when getting the Department
            var department = _giraf._context.Departments
                .Where(dep => dep.Key == id);

            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstOrDefaultAsync();

            if (depa == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.NotFound);

            var members = DepartmentDTO.FindMembers(depa.Members, _roleManager, _giraf);
            return new Response<DepartmentDTO>(new DepartmentDTO(depa, members));
        }

        /// <summary>
        /// Gets the citizen names.
        /// </summary>
        /// <returns>The citizen names else DepartmentNotFound, NotAuthorised or DepartmentHasNoCitizens</returns>
        /// <param name="id">Id of <see cref="Department"/> to get citizens for</param>
        [HttpGet("{id}/citizens")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        public async Task<Response<List<UserNameDTO>>> GetCitizenNamesAsync(long id)
        {
            var department = _giraf._context.Departments.FirstOrDefault(dep => dep.Key == id);

            if (department == null) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentNotFound);

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

           currentUser =  _giraf._context.Users.Include(a => a.Department)
                                               .FirstOrDefault(d => d.UserName == currentUser.UserName);
                  
            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != department?.Key && !isSuperUser)
                return new ErrorResponse<List<UserNameDTO>>(ErrorCode.NotAuthorized);

            // Get all citizens
            var roleCitizenId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Citizen)
                                                     .Select(c => c.Id).FirstOrDefault();

            if (roleCitizenId == null) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentHasNoCitizens);

            // get all users where id of role is in roleCitizenId
            var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleCitizenId)
                                .Select(r => r.UserId).Distinct();

            if (!userIds.Any()) return new ErrorResponse<List<UserNameDTO>>(ErrorCode.DepartmentHasNoCitizens);

            // get a list of the name of all citizens in the department
            var usersNamesInDepartment = _giraf._context.Users
                .Where(u => userIds.Any(ui => ui == u.Id) && u.DepartmentKey == department.Key)
                .Select(u =>
                    new UserNameDTO(u.UserName, GirafRoles.Citizen, u.Id)
                ).ToList();

            return new Response<List<UserNameDTO>>(usersNamesInDepartment);
        }

        /// <summary>
        /// Create a new department. it's only necesary to supply the departments name
        /// </summary>
        /// <param name="depDTO">The <see cref="DepartmentDTO"/> to create
        /// </param>
        /// <returns>The new departmentDTO with all database-generated information if success 
        /// else: MissingProperties, UserNotFound, NotAuthorised, InvalidProperties or CouldNotCreateDepartmentUser
        /// </returns>
        [HttpPost("")]
        [Authorize]
        public async Task<Response<DepartmentDTO>> Post([FromBody]DepartmentDTO depDTO)
        {
            try
            {
                if (depDTO?.Name == null)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.MissingProperties,
                        "Deparment name has to be specified!");

                var authenticatedUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
                
                if(authenticatedUser == null)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
                
                var userRole = await _roleManager.findUserRole(_giraf._userManager, authenticatedUser);

                if (userRole != GirafRoles.SuperUser)
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

                _giraf._context.Departments.Add(department);
                _giraf._context.SaveChanges();
                
                //Create a new user with the supplied information

                var departmentUser = new GirafUser(depDTO.Name, department, GirafRoles.Department) {IsDepartment = true};

                //department.Members.Add(user);
                
                var identityUser = await _giraf._userManager.CreateAsync(departmentUser, "0000");
                
                if (identityUser.Succeeded == false)
                    return new ErrorResponse<DepartmentDTO>(ErrorCode.CouldNotCreateDepartmentUser, string.Join("\n", identityUser.Errors));

                await _giraf._userManager.AddToRoleAsync(departmentUser, GirafRole.Department);

                //Save the changes and return the entity
                await _giraf._context.SaveChangesAsync();

                var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
                return new Response<DepartmentDTO>(new DepartmentDTO(department, members));
            }
            catch (System.Exception e)
            {
                var errorDescription = $"Exception in Post: {e.Message}, {e.InnerException}";
                _giraf._logger.LogError(errorDescription);
                return new ErrorResponse<DepartmentDTO>(ErrorCode.Error, errorDescription);
            }
        }

        /// <summary>
        /// Add a user that does not have a department to the given department.
        /// Requires role Department, Guardian or SuperUser
        /// </summary>
        /// <param name="departmentId">Identifier for the <see cref="Department"/>to add user to</param>
        /// <param name="userId">The ID of a <see cref="GirafUser"/> to be added to the department.</param>
        /// <returns>Else: MissingProperties, UserNotFound, NotAuthorised, DepartmentUserNotFound, 
        /// UserNameAlreadyTakenWithinDepartment, UserAlreadyHasDepartment, or Forbidden </returns>
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

            var dep = await _giraf._context.Departments
                .Where(d => d.Key == departmentId)
                .Include(d => d.Members)
                .Include(d => d.Resources)
                .FirstOrDefaultAsync();

            if (dep == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

            //Check if the user is already in the department
            if (dep.Members.Any(u => u.Id == userId))
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNameAlreadyTakenWithinDepartment);


            var user = await _giraf._context.Users.Where(u => u.Id == userId).Include(u => u.Department)
                                   .FirstOrDefaultAsync();

            var RoleOfUserToAdd = await _roleManager.findUserRole(_giraf._userManager, user);

            // only makes sense to add a guardian or citizen to a department
            if (RoleOfUserToAdd == GirafRoles.SuperUser || RoleOfUserToAdd == GirafRoles.Department) 
                return new ErrorResponse<DepartmentDTO>(ErrorCode.Forbidden); 

            if (user == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserNotFound);
            
            if (user.Department != null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.UserAlreadyHasDepartment);

            user.DepartmentKey = dep.Key;
            dep.Members.Add(user);
            await _giraf._context.SaveChangesAsync();

            var members = DepartmentDTO.FindMembers(dep.Members, _roleManager, _giraf);
            return new Response<DepartmentDTO>(new DepartmentDTO(dep, members));
        }

        /// <summary>
        /// Add a resource to the given department. After this call, the department owns the resource and it is available to all its members.
        /// </summary>
        /// <param name="departmentId">Id of the <see cref="Department"/> to add the resource to.</param>
        /// <param name="resourceId">Id of the <see cref="Pictogram"/> to add to the department.</param>
        /// <returns> The DepartmentDTO represented the updated state of the department if there were no errors.
        /// Else: DepartmentNotFound, ResourceNotFound, NotAuthorized or DepartmentAlreadyOwnsResourc
        /// </returns>
        [HttpPost("{departmentId}/resource/{resourceId}")]
        [Authorize]
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        public async Task<Response<DepartmentDTO>> AddResource(long departmentId, long resourceId)
        {
            var usr = await _giraf.LoadUserWithResources(HttpContext.User);

            //Fetch the department and check that it exists no need to load ressources already on user
            var department = await _giraf._context.Departments.Where(d => d.Key == departmentId)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync();
            
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
                                                  && depres.PictogramKey == resourceId)
                .AnyAsync();

            if (alreadyOwned)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentAlreadyOwnsResource);

            //Remove resource from user
            var usrResource = await _giraf._context.UserResources
                                          .Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == usr.Id)
                                          .FirstOrDefaultAsync();

            if (usrResource == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.ResourceNotFound);

            usr.Resources.Remove(usrResource);
            await _giraf._context.SaveChangesAsync();

            //Change resource AccessLevel to Protected from Private
            resource.AccessLevel = AccessLevel.PROTECTED;

            //Create a relationship between the department and the resource.
            var dr = new DepartmentResource(usr.Department, resource);
            await _giraf._context.DepartmentResources.AddAsync(dr);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
            return new Response<DepartmentDTO>(new DepartmentDTO(usr.Department, members));
        }

        [HttpPut("{departmentId}/name")]
        [Authorize]
        public async Task<Response> ChangeDepartmentName(long departmentId, [FromBody] DepartmentNameDTO nameDTO)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return new ErrorResponse(ErrorCode.NotAuthorized);

            var department = _giraf._context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return new ErrorResponse(ErrorCode.DepartmentNotFound);

            if(nameDTO == null || string.IsNullOrEmpty(nameDTO.Name))
                return new ErrorResponse(ErrorCode.MissingProperties, "Name");

            department.Name = nameDTO.Name;

            _giraf._context.SaveChanges();
            
            return new Response();
        }

        /// <summary>
        /// Endpoint for deleting the <see cref="Department"/> with the given id
        /// </summary>
        /// <returns>Empty response on success else: NotAuthorised or DepartmentNotFound</returns>
        /// <param name="departmentId">Identifier of <see cref="Department"/> to delete</param>
        [HttpDelete("{departmentId}")]
        [Authorize]
        public async Task<Response> DeleteDepartment(long departmentId)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return new ErrorResponse(ErrorCode.NotAuthorized);

            var department = _giraf._context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return new ErrorResponse(ErrorCode.DepartmentNotFound);

            _giraf._context.Remove(department);
            _giraf._context.SaveChanges();
            
            return new Response();
        }

        /// <summary>
        /// Removes a resource from the users department.
        /// </summary>
        /// <param name="resourceId">identifier of <see cref="Pictogram"/></param>
        /// <returns> <see cref="DepartmentDTO"/> of updated state if no problems occured.
        /// Else: ResourceNotFound, NotAuthorized or ResourceNotOwnedByDepartment
        /// </returns>
        [HttpDelete("resource/{resourceId}")]
        [Authorize]
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        public async Task<Response<DepartmentDTO>> RemoveResource(long resourceId)
        {
            //Fetch the department and check that it exists.
            var usr = await _giraf.LoadUserWithResources(HttpContext.User);

            //Fetch the department and check that it exists. No need to fetch dep ressources they are already on user
            var department = await _giraf._context.Departments.Where(d => d.Key == usr.DepartmentKey)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync();

            if (department == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.DepartmentNotFound);

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
                                         .Where(dr => dr.PictogramKey == resource.Id && dr.OtherKey == department.Key)
                .FirstOrDefaultAsync();

            if (drrelation == null)
                return new ErrorResponse<DepartmentDTO>(ErrorCode.ResourceNotOwnedByDepartment);

            usr.Department.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
            return new Response<DepartmentDTO>(new DepartmentDTO(usr.Department, members));
        }
    }
}
