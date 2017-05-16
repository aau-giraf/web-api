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
    /// <summary>
    /// The WeekController allows the user to view and update his week schedule along with deleting it.
    /// </summary>
    [Route("[controller]")]
    public class WeekController : Controller
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
        public WeekController(IGirafService giraf, ILoggerFactory loggerFactory)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Week");
        }		

        /// <summary>
        /// Gets all week schedule for the currently authenticated user.
        /// </summary>
        /// <returns>Ok along with the week schedules, or NotFound if there is no such user or if there are no weeks.</returns>
        [HttpGet]	
        [Authorize]
        public async Task<IActionResult> ReadWeekSchedules()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if(user != null && user.WeekSchedule != null && user.WeekSchedule.Any()){
                return Ok(user.WeekSchedule.Select(w => new WeekDTO(w)));
            }
            else
                return NotFound();
        }

        /// <summary>
        /// Gets the schedule with the specified id.
        /// </summary>
        /// <param name="id">The id of the week schedule to fetch.</param>
        /// <returns>NotFound if the user does not have a week with the given id or
        /// Ok and a serialized version of the week if he does.</returns>
        [HttpGet("{id}")]	
        [Authorize]
        public async Task<IActionResult> ReadUsersWeekSchedule(int id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user.WeekSchedule.Where(w => w.Id == id).FirstOrDefault();
            if (week != null)
                return Ok(new WeekDTO(week));
            else
                return NotFound();
        }

        /// <summary>
        /// Updates one of the days of the week with the given id.
        /// </summary>
        /// <param name="id">The id of the week to update a day for.</param>
        /// <param name="newDay">A serialized version of the day to update.</param>
        /// <returns>NotFound if no week with the given id is owned by the user or 
        /// Ok if everything goes well.</returns>
        [HttpPut("day/{id}")]
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
            return NotFound();
        }
        
        /// <summary>
        /// Updates the entire information of the week with the given id.
        /// </summary>
        /// <param name="id">If of the week to update information for.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns>NotFound if the user does not have a week schedule or
        /// Ok and a serialized version of the updated week if everything went well.
        /// BadRequest if the body of the request does not contain a Week
        /// NotFound if there exists no week with the given Id</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateWeek(int id, [FromBody]WeekDTO newWeek)
        {
            
            if(newWeek == null || newWeek.Id == null) return BadRequest("The body of the request must contain a Week");
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if(user.WeekSchedule.Where(w => w.Id == id).Any())
            {
                var week = user.WeekSchedule.Where(w => w.Id == id).FirstOrDefault();
                if (week == null) return NotFound();
                week.Merge(newWeek);
                await _giraf._context.SaveChangesAsync();
                return Ok(user.WeekSchedule.Select(w => new WeekDTO(w)));
            }
            else    
                return NotFound();
        }

        /// <summary>
        /// Creates an entirely new week for the current user.
        /// </summary>
        /// <param name="newWeek">A serialized version of the new week.</param>
        /// <returns>Ok, along with a list of all the current users week schedules or BadRequest if no valid Week was
        /// found in the request body.</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWeek([FromBody]WeekDTO newWeek)
        {
            if (newWeek == null || newWeek.Id == null) return BadRequest("Failed to find a valid Week in the request body.");
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Add(new Week(newWeek));
            await _giraf._context.SaveChangesAsync();
            return Ok(user.WeekSchedule.Select(w => new WeekDTO(w)));
        }

        /// <summary>
        /// Deletes the entire week with the given id.
        /// </summary>
        /// <param name="id">If of the week to delete.</param>
        /// <returns>NotFound if the user does not have a week schedule or
        /// Ok and a serialized version of the updated week if everything went well.</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteWeek(int id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);

            if(user.WeekSchedule.Where(w => w.Id == id).Any())
            {
                var week = user.WeekSchedule.Where(w => w.Id == id).FirstOrDefault();
                if (week == null) return NotFound();
                user.WeekSchedule.Remove(week);
                await _giraf._context.SaveChangesAsync();
                return Ok(user.WeekSchedule.Select(w => new WeekDTO(w)));
            }
            else    
                return NotFound();
        }
    }
}