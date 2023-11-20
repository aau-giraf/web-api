using GirafEntities.User;
using GirafRepositories.Interfaces;
using GirafRest.Interfaces;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public readonly RoleManager<GirafRole> _roleManager;
        public DepartmentDTO _departmentdto { get; set; } = new DepartmentDTO();
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IGirafUserRepository _userRepository;
        private readonly IGirafRoleRepository _roleRepository;
        private readonly IPictogramRepository _pictogramRepository;

        /// <summary>
        /// Initializes new DepartmentController, injecting services
        /// </summary>
        /// <param name="giraf">Injection of GirafService</param>
        /// <param name="loggerFactory">Injection of logger</param>
        /// <param name="roleManager">Injection of RoleManager</param>
        /// <param name="userRepository">User injection</param>
        /// <param name="departmentRepository">Department Injection</param>
        /// <param name="girafRoleRepository">GIRAF Role Injection</param>
        /// <param name="pictogramRepository">Pictogram Injection</param>
        public DepartmentController(IGirafService giraf,
            ILoggerFactory loggerFactory,
            RoleManager<GirafRole> roleManager,
            IGirafUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            IGirafRoleRepository girafRoleRepository,
            IPictogramRepository pictogramRepository)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Department");
            _roleManager = roleManager;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _roleRepository = girafRoleRepository;
            _pictogramRepository = pictogramRepository;
        }

        /// <summary>
        /// Get request for getting all the department names.
        /// </summary>
        /// <returns> A list of department names, returns NotFound if no departments in the system</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessResponse<List<DepartmentNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult Get()
        {
            var departmentNameDTOs = _departmentRepository.GetDepartmentNames();

            if (departmentNameDTOs.Result.Count == 0)
            {
                return this.ResourceNotFound(nameof(DepartmentDTO), departmentNameDTOs);
            }

            return Ok(new SuccessResponse<List<DepartmentNameDTO>>(departmentNameDTOs.Result));
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
            {
                return this.ResourceNotFound(nameof(User));
            }

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != id && !isSuperUser)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            }

            var department = _departmentRepository.GetDepartmentMembers(id);

            if (department == null)
            {
                return this.ResourceNotFound(nameof(Department));
            }

            var members = _departmentdto.FindMembers(department.Result.Members, _roleManager, _giraf);
            return Ok(new SuccessResponse<DepartmentDTO>(new DepartmentDTO(department.Result, members)));
        }

        /// <summary>
        /// Gets the citizen names.
        /// </summary>
        /// <returns>The citizen names else DepartmentNotFound, NotAuthorised or DepartmentHasNoCitizens</returns>
        /// <param name="id">Id of <see cref="Department"/> to get citizens for</param>
        [HttpGet("{id}/citizens")]
        [ProducesResponseType(typeof(SuccessResponse<List<DisplayNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetCitizenNamesAsync(long id)
        {
            var department = _departmentRepository.GetDepartmentById(id);

            if (department == null)
            {
                return this.ResourceNotFound(nameof(Department), department, id);
            }

            var currentUser = await _giraf._userManager.GetUserAsync(HttpContext.User);

            currentUser = _departmentRepository.GetUserByDepartment(department, currentUser);

            var isSuperUser = await _giraf._userManager.IsInRoleAsync(currentUser, GirafRole.SuperUser);

            if (currentUser?.DepartmentKey != department?.Key && !isSuperUser)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));
            }

            // Get all citizens
            var roleCitizenId = _departmentRepository.GetCitizenRoleID();

            if (roleCitizenId == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, "Department has no citizens"));
            }

            // get all users where id of role is in roleCitizenId
            var userIds = _departmentRepository.GetUsersWithRoleID(roleCitizenId);

            if (!userIds.Any())
            {
                return NotFound(new ErrorResponse(ErrorCode.DepartmentHasNoCitizens, "Department has no citizens"));
            }

            // get a list of the name of all citizens in the department
            var citizens = _roleRepository.GetAllCitizens();
            var usersNamesInDepartment = _userRepository.GetUsersInDepartment(department.Key, citizens);
            var girafToDisplayNameDTO = usersNamesInDepartment.Select(u => new DisplayNameDTO(u.DisplayName, GirafRoles.Citizen, u.Id)).ToList();
            return Ok(new SuccessResponse<List<DisplayNameDTO>>(girafToDisplayNameDTO));
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
        [ProducesResponseType(typeof(SuccessResponse<DepartmentDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] DepartmentDTO depDTO)
        {
            try
            {
                if (depDTO?.Name == null)
                {
                    return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Deparment name has to be specified!"));
                }

                var authenticatedUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
                if (authenticatedUser == null)
                {
                    return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
                }

                var userRole = await _departmentRepository.GetUserRole(_roleManager, _giraf._userManager, authenticatedUser);
                if (userRole != GirafRoles.SuperUser)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User is not a super user"));
                }

                //Add the department to the database.
                Department department = new Department(depDTO);

                //Add all members specified by either id or username in the DTO
                if (depDTO.Members != null)
                {
                    foreach (var mem in depDTO.Members)
                    {
                        var usr = await _userRepository.GetUserWithId(mem.UserId);
                        if (usr == null)
                        {
                            return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "The member list contained an invalid id: " + mem));
                        }

                        department.Members.Add(usr);
                        usr.Department = department;
                    }
                }

                //Add all the resources with the given ids
                if (depDTO.Resources != null)
                {
                    foreach (var reso in depDTO.Resources)
                    {
                        var res = await _pictogramRepository.GetPictogramWithID(reso);
                        if (res == null)
                        {
                            return BadRequest(new ErrorResponse(
                                ErrorCode.InvalidProperties,
                                "The list of resources contained an invalid resource id: " + reso
                            ));
                        }

                        var dr = new DepartmentResource(department, res);
                        await _departmentRepository.AddDepartmentResource(dr);
                    }
                }
                await _departmentRepository.AddDepartment(department);
         

                //Create a new user with the supplied information

                var departmentUser = new GirafUser(depDTO.Name, depDTO.Name, department, GirafRoles.Department) { IsDepartment = true };

                var identityUser = await _giraf._userManager.CreateAsync(departmentUser, "0000");

                if (identityUser.Succeeded == false)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.CouldNotCreateDepartmentUser, string.Join("\n", identityUser.Errors)));
                }

                await _giraf._userManager.AddToRoleAsync(departmentUser, GirafRole.Department);

                //Save the changes and return the entity
                await _departmentRepository.Update(department);
                var members = _departmentdto.FindMembers(department.Members, _roleManager, _giraf);
                return CreatedAtRoute("GetDepartment", new { id = department.Key }, new SuccessResponse<DepartmentDTO>(new DepartmentDTO(department, members)));
            }
            catch (Exception e)
            {
                var errorDescription = $"Exception in Post: {e.Message}, {e.InnerException}";
                _giraf._logger.LogError(errorDescription);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse(ErrorCode.Error, errorDescription));
            }
        }

        /// <summary>
        /// Handles changing name of a Department.
        /// </summary>
        /// <param name="departmentId">id of department to change</param>
        /// <param name="nameDTO">Object handling new name.</param>
        /// <returns>Returns empty ok response.</returns>
        [HttpPut("{departmentId}/name")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ChangeDepartmentName(long departmentId, [FromBody] DepartmentNameDTO nameDTO)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);

            var department = _departmentRepository.GetDepartmentById(departmentId);
            
            if (department == null)
            {
                return this.ResourceNotFound(nameof(Department),  departmentId);
            }

            if (nameDTO == null || string.IsNullOrEmpty(nameDTO.Name))
            {
                return this.MissingPropertyFromRequest(nameof(nameDTO.Name));
            }

            department.Name = nameDTO.Name;

            await _departmentRepository.Update(department);

            return Ok(new SuccessResponse("Name of department changed"));
        }

        /// <summary>
        /// Endpoint for deleting the <see cref="Department"/> with the given id
        /// </summary>
        /// <returns>Empty response on success else: NotAuthorised or DepartmentNotFound</returns>
        /// <param name="departmentId">Identifier of <see cref="Department"/> to delete</param>
        [HttpDelete("{departmentId}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDepartment(long departmentId)
        {
            var requestingUser = await _giraf.LoadBasicUserDataAsync(HttpContext.User);

            var department = _departmentRepository.GetDepartmentById(departmentId);

            if (department == null)
            {
                return this.ResourceNotFound(nameof(Department), departmentId);
            }

            await _departmentRepository.RemoveDepartment(department);

            return Ok(new SuccessResponse("Department deleted"));
        }
    }
}