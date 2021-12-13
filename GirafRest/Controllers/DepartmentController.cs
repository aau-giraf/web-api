using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Enums;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Handles Department endpoints
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class DepartmentController : Controller
    {
        private readonly IGirafService _giraf;

        private readonly RoleManager<GirafRole> _roleManager;

        private readonly IAuthenticationService _authentication;

        // SHOULD BE REMOVED AFTER REFACTORING OF THIS CONTROLLER HAS BEEN COMPLETED!
        private readonly GirafDbContext _context;

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
            IAuthenticationService authentication,
            GirafDbContext context)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
            _roleManager = roleManager;
            _authentication = authentication;
            _context = context;
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
            var departmentNameDTOs = await _context.Departments.Select(d => new DepartmentNameDTO(d.Key, d.Name)).ToListAsync();

            if (departmentNameDTOs.Count == 0)
                return NotFound(new ErrorResponse(ErrorCode.NotFound, "No departments found"));

            return Ok(new SuccessResponse<List<DepartmentNameDTO>>(departmentNameDTOs));
        }

        /// <summary>
        /// Get the department with the specified id.
        /// </summary>
        /// <param name="id">The id of the <see cref="Department"/> to retrieve.</param>
        /// <returns>The department as a DepartmentDTO if success else UserNotfound, NotAuthorised or NotFound</returns>
        [HttpGet("{id}", Name = "GetDepartment")]
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Get(long id)
        {
            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            if (currentUser == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != id && !isSuperUser)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            //.Include is used to get information on members aswell when getting the Department
            var department = _context.Departments
                .Where(dep => dep.Key == id);

            var depa = await department
                .Include(dep => dep.Members)
                .Include(dep => dep.Resources)
                .FirstOrDefaultAsync();

            if (depa == null)
                return NotFound(new ErrorResponse(ErrorCode.NotFound, "Department not found"));

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
            var department = _context.Departments.FirstOrDefault(dep => dep.Key == id);

            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found"));

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            currentUser = _context.Users.Include(a => a.Department)
                                               .FirstOrDefault(d => d.UserName == currentUser.UserName);

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != department?.Key && !isSuperUser)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            // Get all citizens
            var roleCitizenId = _context.Roles.Where(r => r.Name == GirafRole.Citizen)
                                                     .Select(c => c.Id).FirstOrDefault();

            if (roleCitizenId == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, "Department has no citizens"));

            // get all users where id of role is in roleCitizenId
            var userIds = _context.UserRoles.Where(u => u.RoleId == roleCitizenId)
                                .Select(r => r.UserId).Distinct();

            if (!userIds.Any())
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, "Department has no citizens"));

            // get a list of the name of all citizens in the department
            var usersNamesInDepartment = _context.Users
                .Where(u => userIds.Any(ui => ui == u.Id) && u.DepartmentKey == department.Key)
                .Select(u =>
                    new DisplayNameDTO(u.DisplayName, GirafRoles.Citizen, u.Id)
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
                        new ErrorResponse(ErrorCode.MissingProperties, "Deparment name has to be specified!"));

                var authenticatedUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
                if (authenticatedUser == null)
                    return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

                var userRole = await _roleManager.findUserRole(_giraf._userManager, authenticatedUser);
                if (userRole != GirafRoles.SuperUser)
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User is not a super user"));

                //Add the department to the database.
                Department department = new Department(depDTO);

                //Add all members specified by either id or username in the DTO
                if (depDTO.Members != null)
                {
                    foreach (var mem in depDTO.Members)
                    {
                        var usr = await _context.Users
                            .Where(u => u.UserName == mem.DisplayName || u.Id == mem.UserId)
                            .FirstOrDefaultAsync();
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
                        var res = await _context.Pictograms
                            .Where(p => p.Id == reso)
                            .FirstOrDefaultAsync();
                        if (res == null)
                            return BadRequest(new ErrorResponse(
                                ErrorCode.InvalidProperties,
                                "The list of resources contained an invalid resource id: " + reso
                            ));
                        var dr = new DepartmentResource(department, res);
                        await _context.DepartmentResources.AddAsync(dr);
                    }
                }

                _context.Departments.Add(department);
                _context.SaveChanges();

                //Create a new user with the supplied information

                var departmentUser = new GirafUser(depDTO.Name, depDTO.Name, department, GirafRoles.Department) { IsDepartment = true };

                //department.Members.Add(user);

                var identityUser = await _giraf._userManager.CreateAsync(departmentUser, "0000");

                if (identityUser.Succeeded == false)
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.CouldNotCreateDepartmentUser, string.Join("\n", identityUser.Errors)));

                await _giraf._userManager.AddToRoleAsync(departmentUser, GirafRole.Department);

                //Save the changes and return the entity
                await _context.SaveChangesAsync();

                var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
                return CreatedAtRoute(
                    "GetDepartment",
                    new { id = department.Key },
                    new SuccessResponse<DepartmentDTO>(new DepartmentDTO(department, members)));
            }
            catch (System.Exception e)
            {
                var errorDescription = $"Exception in Post: {e.Message}, {e.InnerException}";
                _giraf._logger.LogError(errorDescription);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, errorDescription));
            }
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
            var usr = await _giraf.LoadUserWithResources(HttpContext.User);

            //Fetch the department and check that it exists no need to load ressources already on user
            var department = await _context.Departments.Where(d => d.Key == departmentId)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync();
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found"));


            //Fetch the resource with the given id, check that it exists and that the user owns it.
            var resource = await _context.Pictograms.Where(f => f.Id == resourceId).FirstOrDefaultAsync();
            if (resource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, "Resource not found"));

            var resourceOwned = await _giraf.CheckPrivateOwnership(resource, usr);
            if (!resourceOwned)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not own the resource"));

            //Check if the department already owns the resource
            var alreadyOwned = await _context.DepartmentResources
                                           .Where(depres => depres.OtherKey == departmentId
                                                  && depres.PictogramKey == resourceId)
                .AnyAsync();
            if (alreadyOwned)
                return BadRequest(new ErrorResponse(ErrorCode.DepartmentAlreadyOwnsResource, "Resource is already in department"));

            //Remove resource from user
            var usrResource = await _context.UserResources
                                          .Where(ur => ur.PictogramKey == resource.Id && ur.OtherKey == usr.Id)
                                          .FirstOrDefaultAsync();
            if (usrResource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, "Resource not found"));

            usr.Resources.Remove(usrResource);
            await _context.SaveChangesAsync();

            //Change resource AccessLevel to Protected from Private
            resource.AccessLevel = AccessLevel.PROTECTED;

            //Create a relationship between the department and the resource.
            var dr = new DepartmentResource(usr.Department, resource);
            await _context.DepartmentResources.AddAsync(dr);
            await _context.SaveChangesAsync();

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
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            var department = _context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found"));

            if (nameDTO == null || string.IsNullOrEmpty(nameDTO.Name))
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Name is missing"));

            department.Name = nameDTO.Name;

            _context.SaveChanges();

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
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (!_authentication.HasEditDepartmentAccess(requestingUser, departmentId).Result)
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            var department = _context.Departments
                .FirstOrDefault(d => d.Key == departmentId);
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found"));

            _context.Remove(department);
            _context.SaveChanges();

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
            var usr = await _giraf.LoadUserWithResources(HttpContext.User);

            //Fetch the department and check that it exists. No need to fetch dep ressources they are already on user
            var department = await _context.Departments.Where(d => d.Key == usr.DepartmentKey)
                                         .Include(d => d.Members)
                                         .FirstOrDefaultAsync();
            if (department == null)
                return NotFound(new ErrorResponse(ErrorCode.DepartmentNotFound, "Department not found"));

            //Fetch the resource with the given id, check that it exists.
            var resource = await _context.Pictograms
                .Where(f => f.Id == resourceId)
                .FirstOrDefaultAsync();
            if (resource == null)
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, "Resource not found"));

            var resourceOwned = await _giraf.CheckProtectedOwnership(resource, usr);
            if (!resourceOwned)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.NotAuthorized, "User does not own resource"));

            //Check if the department already owns the resource and remove if so.
            var drrelation = await _context.DepartmentResources
                                         .Where(dr => dr.PictogramKey == resource.Id && dr.OtherKey == department.Key)
                .FirstOrDefaultAsync();
            if (drrelation == null)
                return StatusCode(StatusCodes.Status403Forbidden,
                    new ErrorResponse(ErrorCode.ResourceNotOwnedByDepartment, "Resource not owned by department"));

            usr.Department.Resources.Remove(drrelation);
            await _context.SaveChangesAsync();

            //Return Ok and the department - the resource is now visible in deparment.Resources
            var members = DepartmentDTO.FindMembers(department.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(usr.Department, members)));
        }
    }
}
