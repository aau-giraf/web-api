using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GirafRest.Data;
using GirafRest.Models;
using Microsoft.AspNetCore.Authorization;
using GirafRest.Models.DTOs;
using GirafRest.Services;
using Microsoft.Extensions.Logging;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    [Authorize]
    [Route("v2/api/[controller]")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IAuthenticationService _authentication;
        private readonly IGirafService _giraf;

        public ActivityController(IGirafService giraf, ILoggerFactory loggerFactory, IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Activity");
            _authentication = authentication;
        }

        // POST: api/Activity
        [HttpPost("{userId}/{weekplanName}/{weekYear}/{weekNumber}/{weekDay}")]
        [Authorize]
        public async Task<Response<ActivityDTO>> PostActivity([FromBody] ActivityDTO activity, string userId, string weekplanName, int weekYear, int weekNumber, Days weekDay)
        {
            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<ActivityDTO>(errorCode: ErrorCode.NotAuthorized);

            var dbWeek = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (dbWeek != null)
                return new ErrorResponse<ActivityDTO>(errorCode: ErrorCode.WeekNotFound);

            Weekday dbWeekDay = dbWeek.Weekdays.First(day => day.Day == weekDay);

            int order = dbWeekDay.Activities.Max(act => act.Order);
            order++;

            Activity dbActivity = new Activity(dbWeekDay, new Pictogram() { Id=activity.Pictogram.Id}, order, ActivityState.Normal);
            _giraf._context.Activities.Add(dbActivity);
            await _giraf._context.SaveChangesAsync();


            return new Response<ActivityDTO>(new ActivityDTO(dbActivity));
        }

        // DELETE: api/Activity/5
        [HttpDelete("{userId}/delete/{activityId}")]
        [Authorize]
        public async Task<Response> DeleteActivity(string userId, long activityId)
        {
            GirafUser user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse(errorCode: ErrorCode.NotAuthorized);

            var activity = await _giraf._context.Activities.FindAsync(activityId);
            if (activity == null)
            {
                return new ErrorResponse(errorCode: ErrorCode.NotFound);
            }

            _giraf._context.Activities.Remove(activity);
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }

        private bool ActivityExists(long id)
        {
            return _giraf._context.Activities.Any(e => e.Key == id);
        }
    }
}
