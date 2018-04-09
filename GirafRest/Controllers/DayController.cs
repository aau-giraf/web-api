using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{    
    [Route("v1/[controller]")]
    public class DayController : Controller
    {
        /// <summary>
        /// A reference to GirafService, that contains common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        /// <summary>
        /// Creates a new DayController, as with all controllers, this is instantiated by ASP.NET and the parametersa re
        /// dependency injected.
        /// </summary>
        /// <param name="giraf"> Reference to the implementation of GirafService </param>
        /// <param name="loggerFactory"> Reference to the logger allowing for uniform logging of all controllers</param>
        public DayController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Day");
        }

        /// <summary>
        /// Updates a specified day of the week with the given id.
        /// </summary>
        /// <param name="id">The id of the week to update a day for.</param>
        /// <param name="newDay">A serialized version of the day to update.</param>
        /// <returns>NotFound if no week with the given id is owned by the user or 
        /// Ok if everything goes well.
        /// BadRequest if the body does not contain a parseable WeekdayDTO</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<Response<WeekDTO>> UpdateDay(long id, [FromBody]WeekdayDTO newDay)    
        {	
            if(newDay == null) return new ErrorResponse<WeekDTO>(ErrorCode.FormatError);
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user?.WeekSchedule.FirstOrDefault(w => w.Id == id);
            if(week != null && week.Weekdays.Any())
            {
                var ressList = new List<Resource>();
                week.Weekdays.Remove(week.Weekdays.First(d => d.Day == newDay.Day));
                foreach (var ress in newDay.Elements )
                {
                    if (ress is ChoiceDTO)
                    {
                        ressList.Add((_giraf._context.Choices.FirstOrDefault(c => c.Id == ress.Id)));
                    }

                    if (ress is PictogramDTO)
                    {
                        ressList.Add((_giraf._context.Pictograms.FirstOrDefault(p => p.Id == ress.Id)));
                    }
                }
                week.Weekdays.Add(new Weekday(newDay.Day, ressList));
                await _giraf._context.SaveChangesAsync();	
                return new Response<WeekDTO>(new WeekDTO(week));	
            }
            return new ErrorResponse<WeekDTO>(ErrorCode.WeekScheduleNotFound);
        }
    }
}