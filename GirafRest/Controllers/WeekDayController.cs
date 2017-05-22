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

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class WeekDayController : Controller
    {
        /// <summary>
        /// A reference to GirafService, that contains common functionality for all controllers.
        /// </summary>
        private readonly IGirafService _giraf;
        public WeekDayController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Day");
        }
        /// <summary>
        /// Updates one of the days of the week with the given id.
        /// </summary>
        /// <param name="id">The id of the week to update a day for.</param>
        /// <param name="newDay">A serialized version of the day to update.</param>
        /// <returns>NotFound if no week with the given id is owned by the user or 
        /// Ok if everything goes well.</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDay(int id, [FromBody]WeekdayDTO newDay)	
        {	
            if(newDay == null) return BadRequest("The body of the request must contain a Weekday");
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user.WeekSchedule.Where(w => w.Id == id).FirstOrDefault();
            if(week != null && week.Weekdays.Any())
            {
                week.Weekdays.Remove(week.Weekdays.Where(d => d.Day == newDay.Day).First());
                week.Weekdays.Add(new Weekday(newDay));
                await _giraf._context.SaveChangesAsync();	
                return Ok(new WeekDTO(week));	
            }
            return NotFound("Specified Week Not Found");
        }
    }
}