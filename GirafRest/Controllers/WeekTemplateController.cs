using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GirafRest.Shared.SharedMethods;
using GirafRest.Data;

namespace GirafRest.Controllers
{
    /// <summary>
    /// WeekTemplateController for CRUD og WeekTemplate
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class WeekTemplateController : Controller
    {
        /// <summary>
        /// A reference to GirafService, that contains common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        
        /// <summary>
        /// reference to the authenticationservice which provides commong authentication checks
        /// </summary>
        private readonly IAuthenticationService _authentication;

        // SHOULD BE REMOVED AFTER REFACTORING OF THIS CONTROLLER HAS BEEN COMPLETED!
        private readonly GirafDbContext _context;


        /// <summary>
        /// Constructor is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        /// <param name="authentication"></param>
        public WeekTemplateController(IGirafService giraf,
            ILoggerFactory loggerFactory,
            IAuthenticationService authentication,
            GirafDbContext context)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("WeekTemplate");
            _authentication = authentication;
            _context = context;
        }

        /// <summary>
        /// Gets all schedule templates for the currently authenticated user.
        /// Available to all users.
        /// </summary>
        /// <returns>NoWeekTemplateFound if there are no templates in the user's department.
        /// OK otherwise.</returns>
        [HttpGet("")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<WeekTemplateNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetWeekTemplates()
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);

            if (!await _authentication.HasTemplateAccess(user))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            var result = _context.WeekTemplates
                             .Where(t => t.DepartmentKey == user.DepartmentKey)
                             .Select(w => new WeekTemplateNameDTO(w.Name, w.Id)).ToArray();

            if (result.Length < 1)
            {
                return NotFound(new ErrorResponse(ErrorCode.NoWeekTemplateFound, "No week template found"));
            }
            else
            {
                return Ok(new SuccessResponse<IEnumerable<WeekTemplateNameDTO>>(result));
            }
        }

        /// <summary>
        /// Gets the week template with the specified id.
        /// Available to all users.
        /// </summary>
        /// <param name="id">The id of the week template to fetch.</param>
        /// <returns>Notfound if there is no template in the authenticated user's department by that ID,
        /// <b>or</b> if user does not have the proper authorisation for the template.</returns>
        [HttpGet("{id}", Name = "GetWeekTemplate")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<WeekTemplateDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetWeekTemplate(long id)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);

            var template = await (_context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(pa => pa.Pictograms)
                .ThenInclude(p => p.Pictogram)
                .Where(t => t.DepartmentKey == user.DepartmentKey)
                .FirstOrDefaultAsync(w => w.Id == id));

            if (template == null)
                return NotFound(new ErrorResponse(ErrorCode.NoWeekTemplateFound, "No week template found"));

            if (!await _authentication.HasTemplateAccess(user, template?.DepartmentKey))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            return Ok(new SuccessResponse<WeekTemplateDTO>(new WeekTemplateDTO(template)));
        }

        /// <summary>
        /// Creates new week template in the department of the given user. 
        /// Available to Supers, Departments and Guardians.
        /// </summary>
        /// <param name="templateDto">After successful execution, a new week template will be created with the same values as this DTO.</param>
        /// <returns>UserMustBeInDepartment if user has no associated department.
        /// MissingProperties if properties are missing.
        /// ResourceNotFound if any pictogram id is invalid.
        /// A DTO containing the full information on the created template otherwise.</returns>
        [HttpPost("")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<WeekTemplateDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateWeekTemplate([FromBody] WeekTemplateDTO templateDto)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);

            if (!await _authentication.HasTemplateAccess(user))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            if (templateDto == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing templateDto"));

            Department department = _context.Departments.FirstOrDefault(d => d.Key == user.DepartmentKey);
            if (department == null)
                return BadRequest(new ErrorResponse(ErrorCode.UserMustBeInDepartment, "User must be in a department"));

            var newTemplate = new WeekTemplate(department);

            var errorCode = await SetWeekFromDTO(templateDto, newTemplate, _giraf);
            if (errorCode != null)
                return BadRequest(errorCode);

            _context.WeekTemplates.Add(newTemplate);
            await _context.SaveChangesAsync();
            return CreatedAtRoute(
                "GetWeekTemplate",
                new { id = newTemplate.Id },
                new SuccessResponse<WeekTemplateDTO>(new WeekTemplateDTO(newTemplate))
            );
        }

        /// <summary>
        /// Overwrite the information of a week template.
        /// Available to all Supers, and to Departments and Guardians of the same department as the template.
        /// </summary>
        /// <param name="id">Id of the template to overwrite.</param>
        /// <param name="newValuesDto">After successful execution, specified template will have the same values as this DTO</param>
        /// <returns> WeekTemplateNotFound if no template exists with the given id.
        /// NotAuthorized if not available to authenticated user(see summary).
        /// MissingProperties if properties are missing.
        /// ResourceNotFound if any pictogram id is invalid.
        /// A DTO containing the full information on the created template otherwise.</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<WeekTemplateDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateWeekTemplate(long id, [FromBody] WeekTemplateDTO newValuesDto)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (user == null)
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            if (newValuesDto == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing newValuesDto"));

            var template = _context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(e => e.Pictograms)
                .FirstOrDefault(t => id == t.Id);

            if (template == null)
                return NotFound(new ErrorResponse(ErrorCode.WeekTemplateNotFound, "Weektemplate not found"));

            if (!await _authentication.HasTemplateAccess(user, template?.DepartmentKey))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            var errorCode = await SetWeekFromDTO(newValuesDto, template, _giraf);
            if (errorCode != null)
                return BadRequest(errorCode);

            _context.WeekTemplates.Update(template);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResponse<WeekTemplateDTO>(new WeekTemplateDTO(template)));
        }

        /// <summary>
        /// Deletes the template of the given ID.
        /// Available to all Supers, and to Departments and Guardians of the same department as the template.
        /// </summary>
        /// <param name="id">Id of the template that will be deleted.</param>
        /// <returns> WeekTemplateNotFound if no template exists with the given id.
        /// NotAuthorized if not available to authenticated user(see summary).
        /// OK otherwise. </returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<ActionResult> DeleteTemplate(long id)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (user == null)
                return Unauthorized(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            var template = _context.WeekTemplates
                                          .FirstOrDefault(t => id == t.Id);

            if (template == null)
                return NotFound(new ErrorResponse(ErrorCode.WeekTemplateNotFound, "Weektemplate not found"));

            if (!await _authentication.HasTemplateAccess(user, template?.DepartmentKey))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            _context.WeekTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return Ok(new SuccessResponse("Deleted week template"));
        }
    }
}
