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

namespace GirafRest.Controllers
{
    [Route("[controller]")]
    public class WeekController : Controller
    {
        private readonly GirafController _giraf;

        public WeekController(GirafDbContext context, UserManager<GirafUser> userManager,	
            ILoggerFactory loggerFactory)
        {
            _giraf = new GirafController(context, userManager, loggerFactory.CreateLogger<PictogramController>());
        }		
        [HttpGet]	
        [Authorize]
        public async Task<IActionResult> GetWeekSchedules()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            //System.Console.WriteLine(user.WeekSchedule.ElementAt(1).Weekdays.Count());
            List<WeekDTO> userWeeks = new List<WeekDTO>();
            foreach(var week in user.WeekSchedule)
            {
                userWeeks.Add(new WeekDTO(week));
            }
            //System.Console.WriteLine(userWeeks[1].Dayzs.Count());
            return Ok(userWeeks);	
        }

        [HttpGet("{id}")]	
        [Authorize]
        public async Task<IActionResult> GetUsersWeekSchedule(int id)
        {
            var user = await _giraf._context.Users.Where(u => u.Id == id.ToString()).FirstAsync();
            var week = user.WeekSchedule.Where(w => w.Id == id).First();
            if(week != null)
                return Ok(new WeekDTO(week));
            else
                return NotFound();	
        }

        [HttpPut("day/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDay(int id, [FromBody]WeekdayDTO newDay)	
        {	
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user.WeekSchedule.Where(w => w.Id == id).First();
            if(week != null && week.Weekdays.Any())
            {
                week.Weekdays.Remove(week.Weekdays.Where(d => d.Day == newDay.Day).First());
                week.Weekdays.Add(new Weekday(newDay));
                user.WeekSchedule.Remove(user.WeekSchedule.Where(w => w.Id == id).First());
                user.WeekSchedule.Add(week);
                await _giraf._context.SaveChangesAsync();	
                return Ok(new WeekDTO(week));	
            }
            else
                return NotFound();
        }
        
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateWeek(int id, [FromBody]WeekDTO newWeek)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Where(w => w.Id == id).First().Merge(newWeek);
            await _giraf._context.SaveChangesAsync();
            List<WeekDTO> userWeeks = new List<WeekDTO>();
            foreach(var week in user.WeekSchedule)
            {
                userWeeks.Add(new WeekDTO(week));
            }
            return Ok(userWeeks);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateWeek([FromBody]WeekDTO newWeek)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Add(new Week(newWeek));
            await _giraf._context.SaveChangesAsync();
            List<WeekDTO> userWeeks = new List<WeekDTO>();
            foreach(var week in user.WeekSchedule)
            {
                userWeeks.Add(new WeekDTO(week));
            }
            return Ok(userWeeks);
        }
    }
}