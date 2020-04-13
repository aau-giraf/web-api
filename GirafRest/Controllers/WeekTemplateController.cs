using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Extensions;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using GirafRest.Models.Responses;
using Microsoft.AspNetCore.Identity;
using static GirafRest.Shared.SharedMethods;

namespace GirafRest.Controllers
{
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


        /// <summary>
        /// Constructor is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        /// <param name="authentication"></param>
        public WeekTemplateController(IGirafService giraf, 
            ILoggerFactory loggerFactory, 
            IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("WeekTemplate");
            _authentication = authentication;
        }

        /// <summary>
        /// Gets all schedule templates for the currently authenticated user.
        /// Available to all users.
        /// </summary>
        /// <returns>NoWeekTemplateFound if there are no templates in the user's department.
        /// OK otherwise.</returns>
        [HttpGet("")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekTemplateNameDTO>>> GetWeekTemplates()
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            
            if (! await _authentication.HasTemplateAccess(user))
                return new ErrorResponse<IEnumerable<WeekTemplateNameDTO>>(ErrorCode.NotAuthorized);
            
            var result = _giraf._context.WeekTemplates
                             .Where(t => t.DepartmentKey == user.DepartmentKey)
                             .Select(w => new WeekTemplateNameDTO(w.Name, w.Id)).ToArray();
            
            if (result.Length < 1)
            {
                return new ErrorResponse<IEnumerable<WeekTemplateNameDTO>>(ErrorCode.NoWeekTemplateFound);
            }
            else
            {
                return new Response<IEnumerable<WeekTemplateNameDTO>>(result);
            }
        }

        /// <summary>
        /// Gets the week template with the specified id.
        /// Available to all users.
        /// </summary>
        /// <param name="id">The id of the week template to fetch.</param>
        /// <returns>Notfound if there is no template in the authenticated user's department by that ID,
        /// <b>or</b> if user does not have the proper authorisation for the template.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<Response<WeekTemplateDTO>> GetWeekTemplate(long id)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            
            var template = await (_giraf._context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                    .ThenInclude(wd => wd.Activities)
                        .ThenInclude(a => a.Pictogram)
                .Where(t => t.DepartmentKey == user.DepartmentKey)
                .FirstOrDefaultAsync(w => w.Id == id));

            if (template == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.NoWeekTemplateFound);

            if (! await _authentication.HasTemplateAccess(user, template?.DepartmentKey) )
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.NotAuthorized);
            
            return new Response<WeekTemplateDTO>(new WeekTemplateDTO(template));
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
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<WeekTemplateDTO>> CreateWeekTemplate([FromBody] WeekTemplateDTO templateDto)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            
            if (! await _authentication.HasTemplateAccess(user))
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.NotAuthorized);

            if(templateDto == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.MissingProperties);

            Department department = _giraf._context.Departments.FirstOrDefault(d => d.Key == user.DepartmentKey);
            if(department == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserMustBeInDepartment);
            
            var newTemplate = new WeekTemplate(department);

            var errorCode = await SetWeekFromDTO(templateDto, newTemplate, _giraf);
            if (errorCode != null)
                return new ErrorResponse<WeekTemplateDTO>(errorCode.ErrorCode, errorCode.ErrorProperties);

            _giraf._context.WeekTemplates.Add(newTemplate);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekTemplateDTO>(new WeekTemplateDTO(newTemplate));
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
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<WeekTemplateDTO>> UpdateWeekTemplate(long id, [FromBody] WeekTemplateDTO newValuesDto)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserNotFound);

            if(newValuesDto == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.MissingProperties);

            var template = _giraf._context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                    .ThenInclude(wd => wd.Activities)
                        .ThenInclude(e => e.Pictogram)
                .FirstOrDefault(t => id == t.Id);

            if (template == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.WeekTemplateNotFound);

            if (! await _authentication.HasTemplateAccess(user, template?.DepartmentKey) )
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.NotAuthorized);
            
            var errorCode = await SetWeekFromDTO(newValuesDto, template, _giraf);
            if (errorCode != null)
                return new ErrorResponse<WeekTemplateDTO>(errorCode.ErrorCode, errorCode.ErrorProperties);

            _giraf._context.WeekTemplates.Update(template);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekTemplateDTO>(new WeekTemplateDTO(template));
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
        [Authorize (Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response> DeleteTemplate(long id)
        {
            var user = await _giraf.LoadBasicUserDataAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserNotFound);

            var template = _giraf._context.WeekTemplates
                                          .FirstOrDefault(t => id == t.Id);

            if (template == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.WeekTemplateNotFound);

            if (! await _authentication.HasTemplateAccess(user, template?.DepartmentKey) )
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.NotAuthorized);

            _giraf._context.WeekTemplates.Remove(template);
            await _giraf._context.SaveChangesAsync();
            return new Response();
        }
    }
}
