using System;
using System.Threading.Tasks;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetWeekSchedule()
        {
            var user = await LoadUserAsync(HttpContext.User);

            return Ok(user.WeekSchedule.days);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDayContent(Days day, Weekday newDay)
        {
            var user = await LoadUserAsync(HttpContext.User);
            user.WeekSchedule.days[(int) day] = newDay;

            await _context.SaveChangesAsync();
            return Ok(user.WeekSchedule.days[(int) day]);
        }

        [HttpPost]
        public async Task<IActionResult> CleanWeek()
        {
            var user = await LoadUserAsync(HttpContext.User);
            Array.Clear(user.WeekSchedule.days, 0, user.WeekSchedule.days.Length);

            await _context.SaveChangesAsync();
            return Ok(user.WeekSchedule.days);
        }
    }
}