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
    public class WeekController : GirafController
    {
        public WeekController(GirafDbContext context, UserManager<GirafUser> userManager,	
            IHostingEnvironment env, ILoggerFactory loggerFactory) 
                : base(context, userManager, env, loggerFactory.CreateLogger<PictogramController>())	
        {	
        }		
        [HttpGet]	
        [Authorize]
        public async Task<IActionResult> GetWeekSchedule()
        {	
            var user = await LoadUserAsync(HttpContext.User);
            return Ok(new WeekDTO(user.WeekSchedule).Days);	
        }	
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateDay(Weekday newDay)	
        {	
            var user = await LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Days.Remove(user.WeekSchedule.Days.Where(d => d.Day == newDay.Day).First());
            user.WeekSchedule.Days.Add(newDay);
            await _context.SaveChangesAsync();	
            return Ok(new WeekDTO(user.WeekSchedule));	
        }
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateWeek(ICollection<Weekday> newWeek)
        {
            var user = await LoadUserAsync(HttpContext.User);
            user.WeekSchedule.Days = newWeek;
            await _context.SaveChangesAsync();
            return Ok(new WeekDTO(user.WeekSchedule));
        }
    }
}