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
    [Route("v2/[controller]")]
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

        /// <summary>
        /// Add a new activity to a given weekplan on the given day.
        /// </summary>
        /// <param name="newActivity">a serialized version of the new activity.</param>
        /// <param name="userId">id of the user that you want to add the activity for.</param>
        /// <param name="weekplanName">name of the weekplan that you want to add the activity on.</param>
        /// <param name="weekYear">year of the weekplan that you want to add the activity on.</param>
        /// <param name="weekNumber">week number of the weekplan that you want to add the activity on.</param>
        /// <param name="weekDay">day of the week that you want to add the activity on.</param>
        /// <returns>Returns <see cref="ActivityDTO"/> for the requested activity on success else MissingProperties, 
        /// UserNotFound, NotAuthorized, WeekNotFound or InvalidDay.</returns>
        [HttpPost("{userId}/{weekplanName}/{weekYear}/{weekNumber}/{weekDay}")]
        [Authorize]
        public async Task<Response<ActivityDTO>> PostActivity([FromBody] ActivityDTO newActivity, string userId, string weekplanName, int weekYear, int weekNumber, Days weekDay)
        {
            if (newActivity == null)
                return new ErrorResponse<ActivityDTO>(ErrorCode.MissingProperties);

            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return new ErrorResponse<ActivityDTO>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<ActivityDTO>(ErrorCode.NotAuthorized);

            var dbWeek = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber && string.Equals(w.Name, weekplanName));
            if (dbWeek == null)
                return new ErrorResponse<ActivityDTO>(ErrorCode.WeekNotFound);

            Weekday dbWeekDay = dbWeek.Weekdays.FirstOrDefault(day => day.Day == weekDay);
            if (dbWeekDay == null)
                return new ErrorResponse<ActivityDTO>(ErrorCode.InvalidDay);

            int order = dbWeekDay.Activities.Select(act => act.Order).DefaultIfEmpty(0).Max();
            order++;

            Activity dbActivity = new Activity(dbWeekDay, new Pictogram() { Id = newActivity.Pictogram.Id }, order, ActivityState.Normal);
            _giraf._context.Activities.Add(dbActivity);
            await _giraf._context.SaveChangesAsync();

            return new Response<ActivityDTO>(new ActivityDTO(dbActivity));
        }

        /// <summary>
        /// Delete an activity with a given id.
        /// </summary>
        /// <param name="userId">id of the user you want to delete an activity for.</param>
        /// <param name="activityId">id of the activity you want to delete.</param>
        /// <returns>Returns success response else UserNotFound, NotAuthorized or ActivityNotFound.</returns>
        [HttpDelete("{userId}/delete/{activityId}")]
        [Authorize]
        public async Task<Response> DeleteActivity(string userId, long activityId)
        {
            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return new ErrorResponse(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse(ErrorCode.NotAuthorized);

            // throws error if none of user's weeks' has the specific activity
            if (!user.WeekSchedule.Any(w => w.Weekdays.Any(wd => wd.Activities.Any(act => act.Key == activityId))))
                return new ErrorResponse(ErrorCode.ActivityNotFound);

            Activity targetActivity = _giraf._context.Activities.First(act => act.Key == activityId);

            _giraf._context.Activities.Remove(targetActivity);
            await _giraf._context.SaveChangesAsync();

            return new Response();
        }
    }
}
