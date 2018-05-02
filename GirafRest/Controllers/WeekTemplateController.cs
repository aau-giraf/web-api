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
        }
    }
}
