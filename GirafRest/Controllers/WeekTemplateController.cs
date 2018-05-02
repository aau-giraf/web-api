using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using GirafRest.Models.Responses;
using static GirafRest.Controllers.SharedMethods;

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
        /// Constructor for the Week-controller. This is called by the asp.net runtime.
        /// </summary>
        /// <param name="giraf">A reference to the GirafService.</param>
        /// <param name="loggerFactory">A reference to an implementation of ILoggerFactory. Used to create a logger.</param>
        public WeekTemplateController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("WeekTemplate");
        }

        /// <summary>
        /// Gets all week schedule for the currently authenticated user.
        /// </summary>
        [HttpGet("")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekTemplateNameDTO>>> GetWeekTemplates()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
            {
                return new ErrorResponse<IEnumerable<WeekTemplateNameDTO>>(ErrorCode.UserNotFound);
            }
            
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
        /// </summary>
        /// <param name="id">The id of the week template to fetch.</param>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<Response<WeekTemplateDTO>> GetWeekTemplate(long id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            
            var week = await (_giraf._context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                    .ThenInclude(wd => wd.Activities)
                .Where(t => t.DepartmentKey == user.DepartmentKey)
                .FirstOrDefaultAsync(w => w.Id == id));
            
            if (week != null)
                return new Response<WeekTemplateDTO>(new WeekTemplateDTO(week));
            else
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.WeekTemplateNotFound);
        }

        [HttpPost("")]
        [Authorize]
        public async Task<Response<WeekTemplateDTO>> CreateWeekTemplate([FromBody] WeekTemplateDTO templateDTO)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserNotFound);
            //TODO: Who is allowed to use this endponit?

            if(templateDTO == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.MissingProperties);

            Department department = _giraf._context.Departments.FirstOrDefault(d => d.Key == user.DepartmentKey);
            if(department == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserMustBeInDepartment);
            
            var newTemplate = new WeekTemplate(department);

            var errorCode = await SetWeekFromDTO(templateDTO, newTemplate, _giraf);
            if (errorCode != null)
                return new ErrorResponse<WeekTemplateDTO>(errorCode.ErrorCode, errorCode.ErrorProperties);

            _giraf._context.WeekTemplates.Add(newTemplate);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekTemplateDTO>(new WeekTemplateDTO(newTemplate));
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<Response<WeekTemplateDTO>> UpdateWeekTemplate(long id, [FromBody] WeekTemplateDTO newValuesDTO)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserNotFound);
            //TODO: Who is allowed to use this endponit?

            if(newValuesDTO == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.MissingProperties);

            var template = _giraf._context.WeekTemplates
                .Include(w => w.Thumbnail)
                .Include(u => u.Weekdays)
                    .ThenInclude(wd => wd.Activities)
                .FirstOrDefault(t => id == t.Id);
            
            if(template == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.WeekTemplateNotFound);
            
            var errorCode = await SetWeekFromDTO(newValuesDTO, template, _giraf);
            if (errorCode != null)
                return new ErrorResponse<WeekTemplateDTO>(errorCode.ErrorCode, errorCode.ErrorProperties);

            _giraf._context.WeekTemplates.Update(template);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekTemplateDTO>(new WeekTemplateDTO(template));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<Response> DeleteTemplate(long id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekTemplateDTO>(ErrorCode.UserNotFound);
            //TODO: Who is allowed to use this endponit?

            var template = _giraf._context.WeekTemplates
                .Include(w => w.Weekdays)
                    .ThenInclude(w => w.Activities)
                .FirstOrDefault(t => id == t.Id);
            
            if(template == null)
                return new ErrorResponse<WeekTemplateDTO>(ErrorCode.WeekTemplateNotFound);

            _giraf._context.WeekTemplates.Remove(template);
            await _giraf._context.SaveChangesAsync();
            return new Response();
        }
    }
}
