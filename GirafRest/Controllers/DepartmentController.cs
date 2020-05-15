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
using Microsoft.AspNetCore.Http;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Handles Department endpoints
    /// </summary>
    [Route("v1/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IGirafService _giraf;

        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        /// <summary>
        /// Initializes new DepartmentController, injecting services
        /// </summary>
        /// <param name="giraf">Injection of GirafService</param>
        /// <param name="loggerFactory">Injection of logger</param>
        /// <param name="roleManager">Injection of RoleManager</param>
        /// <param name="authentication">Injection of Authentication</param>
        public DepartmentController(IGirafService giraf,
            ILoggerFactory loggerFactory,
            RoleManager<GirafRole> roleManager,
            IAuthenticationService authentication)
        {
            if (giraf == null) {
                throw new System.ArgumentNullException(giraf + " is null");
            } else if (loggerFactory == null) {
                throw new System.ArgumentNullException(loggerFactory + " is null");
            }
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
            _roleManager = roleManager;
            _authentication = authentication;
        }

        /// <summary>
        /// Get request for getting all the department names.
        /// </summary>
        /// <returns> A list of department names, returns NotFound if no departments in the system</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessResponse<List<DepartmentNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get()
        {
            var departmentNameDTOs = await _giraf._context.Departments.Select(d => new DepartmentNameDTO(d.Key, d.Name)).ToListAsync().ConfigureAwait(true);

            if (departmentNameDTOs.Count == 0)
                return NotFound(new ErrorResponse(ErrorCode.NotFound, new string("No departments found")));

            return Ok(new SuccessResponse<List<DepartmentNameDTO>>(departmentNameDTOs));
        }

        /// <summary>
        /// Get the department with the specified id.
        /// </summary>
        /// <param name="id">The id of the <see cref="Department"/> to retrieve.</param>
        /// <returns>The department as a DepartmentDTO if success else UserNotfound, NotAuthorised or NotFound</returns>
        [HttpGet("{id}", Name="GetDepartment")]
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(long id)
        {
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true);

            if (currentUser == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new string("User not found")));

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser).ConfigureAwait(true);

            if (currentUser?.DepartmentKey != id && !isSuperUser)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            //.Include is used to get information on members aswell when getting the Department
            var department = _giraf._context.Departments
                .Where(dep => dep.Key == id);

            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);

            if (depa == null)
                return NotFound(new ErrorResponse(ErrorCode.NotFound, new string("Department not found")));

            var members = DepartmentDTO.FindMembers(depa.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(depa, members)));
        }

        /// <summary>
        /// Gets the citizen names.
        /// </summary>
        /// <returns>The citizen names else DepartmentNotFound, NotAuthorised or DepartmentHasNoCitizens</returns>
        /// <param name="id">Id of <see cref="Department"/> to get citizens for</param>
        [HttpGet("{id}/citizens")]
        [Authorize(Roles = GirafRole.SuperUser + "," + GirafRole.Department + "," + GirafRole.Guardian)]
        [ProducesResponseType(typeof(SuccessResponse<List<DisplayNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCitizenNamesAsync(long id)
        {
            var department = _giraf._context.Departments.FirstOrDefault(dep => dep.Key == id);

            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new string("Department not found")));

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true);

            currentUser =  _giraf._context.Users.Include(a => a.Department)
                                               .FirstOrDefault(d => d.UserName == currentUser.UserName);

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser).ConfigureAwait(true);

            if (currentUser?.DepartmentKey != department?.Key && !isSuperUser)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            // Get all citizens
            var roleCitizenId = _giraf._context.Roles.Where(r => r.Name == GirafRole.Citizen)
                                                     .Select(c => c.Id).FirstOrDefault();

            if (roleCitizenId == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, new string("Department has no citizens")));

            // get all users where id of role is in roleCitizenId
            var userIds = _giraf._context.UserRoles.Where(u => u.RoleId == roleCitizenId)
                                .Select(r => r.UserId).Distinct();

            if (!userIds.Any())
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, new string("Department has no citizens")));

            // get a list of the name of all citizens in the department
            var usersNamesInDepartment = _giraf._context.Users
                .Where(u => userIds.Any(ui => ui == u.Id) && u.DepartmentKey == department.Key)
                .Select(u =>
                    new DisplayNameDTO(u.DisplayName, Role.Citizen, u.Id)
                ).ToList();

            return Ok(new SuccessResponse<List<DisplayNameDTO>>(usersNamesInDepartment));
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
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult> Post([FromBody]DepartmentDTO depDTO)
        {
            try
            {
                if (depDTO?.Name == null)
                    return BadRequest(
                        new ErrorResponse(ErrorCode.MissingProperties, new String("Deparment name has to be specified!")));

                var authenticatedUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User).ConfigureAwait(true);
                if (authenticatedUser == null)
                    return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new String("User not found")));

                var userRole = await _roleManager.findUserRole(_giraf._userManager, authenticatedUser).ConfigureAwait(true);
                if (userRole != Role.SuperUser)
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new String("User is not a super user")));

                //Add the department to the database.
                Department department = new Department(depDTO);

                //Add all members specified by either id or username in the DTO
                if (depDTO.Members != null)
                {
                    foreach (var mem in depDTO.Members)
                    {
                        var usr = await _giraf._context.Users
                            .Where(u => u.UserName == mem.DisplayName || u.Id == mem.UserId)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(true);
                        if (usr == null)
                            return BadRequest(new ErrorResponse(
                                ErrorCode.InvalidProperties, "The member list contained an invalid id: " + mem));
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
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(true);
                        if (res == null)
                            return BadRequest(new ErrorResponse(
                                ErrorCode.InvalidProperties, 
                                "The list of resources contained an invalid resource id: " + reso
                            ));
                        var dr = new DepartmentResource(department, res);
                        await _giraf._context.DepartmentResources.AddAsync(dr);
                    }
                }

                _giraf._context.Departments.Add(department);
                _giraf._context.SaveChanges();
                
                //Create a new user with the supplied information

                var departmentUser = new GirafUser(depDTO.Name, depDTO.Name, department, Role.Department) {IsDepartment = true};
                
                //department.Members.Add(user);

                var identityUser = await _giraf._userManager.CreateAsync(departmentUser, "0000").ConfigureAwait(true);

                if (identityUser.Succeeded == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.CouldNotCreateDepartmentUser, string.Join("\n", identityUser.Errors)));

                await _giraf._userManager.AddToRoleAsync(departmentUser, GirafRole.Department).ConfigureAwait(true);

                //Save the changes and return the entity
                await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

                var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
                return CreatedAtRoute(
                    "GetDepartment",
                    new { id = department.Key },
                    new SuccessResponse<DepartmentDTO>(new DepartmentDTO(department, members)));
            } catch(SystemException e) {
                var errorDescription = $"Exception in Post: {e.Message}, {e.InnerException}";
                _giraf._logger.LogError(errorDescription);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, errorDescription)); 
                throw;
            }
        }

        /// <summary>
        /// Add an existing user, that does not have a department, to the given department.
        /// Requires role Department, Guardian or SuperUser
        /// </summary>
        /// <param name="departmentId">Identifier for the <see cref="Department"/>to add user to</param>
        /// <param name="userId">The ID of a <see cref="GirafUser"/> to be added to the department.</param>
        /// <returns>Else: MissingProperties, UserNotFound, NotAuthorised, DepartmentUserNotFound,
        /// UserNameAlreadyTakenWithinDepartment, UserAlreadyHasDepartment, or Forbidden </returns>
        [HttpPost("{departmentId}/user/{userId}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult> AddUser(long departmentId, string userId)
        {
            //Fetch user and department and check that they exist
            if (userId == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new string("Missing userId")));

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User).ConfigureAwait(true);
            if(currentUser == null)
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, new string("No user logged in")));

            var role = await _roleManager.findUserRole(_giraf._userManager, currentUser).ConfigureAwait(true);

            if(role == Role.Department || role == Role.Guardian){
                // lets check that we are in the correct department
                if (currentUser.DepartmentKey != departmentId)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, 
                        new ErrorResponse(ErrorCode.NotAuthorized, new string("Not authorized / department is incorrect")));
                }
            }

            var dep = await _giraf._context.Departments
                .Where(d => d.Key == departmentId)
                .Include(d => d.Members)
                .Include(d => d.Resources)
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);

            if (dep == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new string("Department not found")));

            //Check if the user is already in the department
            if (dep.Members.Any(u => u.Id == userId))
                return Conflict(new ErrorResponse(ErrorCode.UserNameAlreadyTakenWithinDepartment, new string("User is already in department")));


            var user = await _giraf._context.Users.Where(u => u.Id == userId).Include(u => u.Department)
                                   .FirstOrDefaultAsync()
                                   .ConfigureAwait(true);

            var RoleOfUserToAdd = await _roleManager.findUserRole(_giraf._userManager, user).ConfigureAwait(true);

            // only makes sense to add a guardian or citizen to a department
            if (RoleOfUserToAdd == Role.SuperUser || RoleOfUserToAdd == Role.Department)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.Forbidden, new string("Superusers or departments cannot be added to departments")));

            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, new String("User not found")));

            if (user.Department != null)
                return StatusCode(StatusCodes.Status403Forbidden,
                new ErrorResponse(ErrorCode.UserAlreadyHasDepartment, new String("User already in department")));

            user.DepartmentKey = dep.Key;
            dep.Members.Add(user);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            var members = DepartmentDTO.FindMembers(dep.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(dep, members)));
        }

        /// <summary>
        /// Add an existing resource to the given department. After this call, the department owns the resource and it is available to all its members.
        /// </summary>
        /// <param name="departmentId">Id of the <see cref="Department"/> to add the resource to.</param>
        /// <param name="resourceId">Id of the <see cref="Pictogram"/> to add to the department.</param>
        /// <returns> The DepartmentDTO represented the updated state of the department if there were no errors.
        /// Else: DepartmentNotFound, ResourceNotFound, NotAuthorized or DepartmentAlreadyOwnsResourc
        /// </returns>
        [HttpPost("{departmentId}/resource/{resourceId}")]
        [Authorize]
        [Obsolete("Not used by the new WeekPlanner and might need to be changed or deleted (see future works)")]
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddResource(long departmentId, long resourceId)
        {
            var usr = await _giraf.LoadUserWithResources(HttpContext.User).ConfigureAwait(true);

            //Fetch the department and check that it exists no need to load ressources already on user
            var department = await _giraf._context.Departments.Where(d => d.Key == departmentId)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync()
                                         .ConfigureAwait(true);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new string("Department not found")));


            //Fetch the resource with the given id, check that it exists and that the user owns it.
            var resource = await _giraf._context.Pictograms.Where(f => f.Id == resourceId).FirstOrDefaultAsync().ConfigureAwait(true);
            if (resource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, new string("Resource not found")));

            var resourceOwned = await _giraf.CheckPrivateOwnership(resource, usr).ConfigureAwait(true);
            if (!resourceOwned)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not own the resource")));

            //Check if the department already owns the resource
            var alreadyOwned = await _giraf._context.DepartmentResources
                                           .Where(depres => depres.OtherKey == departmentId
                                                  && depres.PictogramKey == resourceId)
                .AnyAsync()
                .ConfigureAwait(true);
            if (alreadyOwned)
                return BadRequest(new ErrorResponse(ErrorCode.DepartmentAlreadyOwnsResource, new string("Resource is already in department")));

            //Remove resource from user
            var usrResource = await _giraf._context.UserResources
                                          .Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == usr.Id)
                                          .FirstOrDefaultAsync()
                                          .ConfigureAwait(true);
            if (usrResource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, new string("Resource not found")));

            usr.Resources.Remove(usrResource);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            //Change resource AccessLevel to Protected from Private
            resource.AccessLevel = AccessLevel.PROTECTED;

            //Create a relationship between the department and the resource.
            var dr = new DepartmentResource(usr.Department, resource);
            await _giraf._context.DepartmentResources.AddAsync(dr);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            //Return Ok and the department - the resource is now visible in deparment.Resources
            var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(usr.Department, members)));
        }

        /// <summary>
        /// Handles changing name of a Department.
        /// </summary>
        /// <param name="departmentId">id of department to change</param>
        /// <param name="nameDTO">Object handling new name.</param>
        /// <returns>Returns empty ok response.</returns>
        [HttpPut("{departmentId}/name")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeDepartmentName(long departmentId, [FromBody] DepartmentNameDTO nameDTO)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User).ConfigureAwait(true);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new String("User does not have permission")));

            var department = _giraf._context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new String("Department not found")));

            if(nameDTO == null || string.IsNullOrEmpty(nameDTO.Name))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, new String("Name is missing")));

            department.Name = nameDTO.Name;

            _giraf._context.SaveChanges();

            return Ok(new SuccessResponse("Name of department changed"));
        }

        /// <summary>
        /// Endpoint for deleting the <see cref="Department"/> with the given id
        /// </summary>
        /// <returns>Empty response on success else: NotAuthorised or DepartmentNotFound</returns>
        /// <param name="departmentId">Identifier of <see cref="Department"/> to delete</param>
        [HttpDelete("{departmentId}")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDepartment(long departmentId)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User).ConfigureAwait(true);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, new string("User does not have permission")));

            var department = _giraf._context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new string("Department not found")));

            _giraf._context.Remove(department);
            _giraf._context.SaveChanges();

            return Ok(new SuccessResponse("Department deleted"));
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
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveResource(long resourceId)
        {
            //Fetch the department and check that it exists.
            var usr = await _giraf.LoadUserWithResources(HttpContext.User).ConfigureAwait(true);

            //Fetch the department and check that it exists. No need to fetch dep ressources they are already on user
            var department = await _giraf._context.Departments.Where(d => d.Key == usr.DepartmentKey)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync()
                                         .ConfigureAwait(true);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, new String("Department not found")));

            //Fetch the resource with the given id, check that it exists.
            var resource = await _giraf._context.Pictograms
                .Where(f => f.Id == resourceId)
                .FirstOrDefaultAsync().ConfigureAwait(true);
            if (resource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, new String("Resource not found")));

            var resourceOwned = await _giraf.CheckProtectedOwnership(resource, usr).ConfigureAwait(true);
            if (!resourceOwned)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.NotAuthorized, new String("User does not own resource")));

            //Check if the department already owns the resource and remove if so.
            var drrelation = await _giraf._context.DepartmentResources
                                         .Where(dr => dr.PictogramKey == resource.Id && dr.OtherKey == department.Key)
                                         .FirstOrDefaultAsync()
                                         .ConfigureAwait(true);
            if (drrelation == null)
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new ErrorResponse(ErrorCode.ResourceNotOwnedByDepartment, new String("Resource not owned by department")));

            usr.Department.Resources.Remove(drrelation);
            await _giraf._context.SaveChangesAsync().ConfigureAwait(true);

            //Return Ok and the department - the resource is now visible in deparment.Resources
            var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(usr.Department, members)));
        }
    }
}
