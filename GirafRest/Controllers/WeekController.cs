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
        public async Task<IActionResult> GetWeekSchedule()
        {	
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            return Ok(new WeekDTO(user.WeekSchedule).Days);	
        }	
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateDay(Weekday newDay)	
        {	
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Days.Remove(user.WeekSchedule.Days.Where(d => d.Day == newDay.Day).First());
            user.WeekSchedule.Days.Add(newDay);
            await _giraf._context.SaveChangesAsync();	
            return Ok(new WeekDTO(user.WeekSchedule));	
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateWeek(ICollection<Weekday> newWeek)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Days = newWeek;
            await _giraf._context.SaveChangesAsync();
            return Ok(new WeekDTO(user.WeekSchedule));
        }
    }
}