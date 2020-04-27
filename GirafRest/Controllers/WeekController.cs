using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using GirafRest.Models.Responses;
using static GirafRest.Shared.SharedMethods;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Controller for managing Weeks, CRUD-ish
    /// </summary>
    [Route("")]
    public class WeekController : Controller
    {
        private readonly IGirafService _giraf;

        private readonly IAuthenticationService _authentication;

        /// <summary>
        /// Constructor for WeekController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
        public WeekController(IGirafService giraf, ILoggerFactory loggerFactory, IAuthenticationService authentication)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Week");
            _authentication = authentication;
        }

        /// <summary>
        /// Gets list of <see cref="WeekDTO"/> for all weeks belonging to the user with the provided id, days not are included
        /// </summary>
        /// <returns>List of <see cref="WeekDTO"/> on success else UserNotFound</returns>
        /// <param name="userId">User identifier for the <see cref="GirafUser" to get schedules for/></param>
        [HttpGet("v2/User/{userId}/week")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekDTO>>> ReadFullWeekSchedules(string userId)
        {
            var user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new ErrorResponse<IEnumerable<WeekDTO>>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<IEnumerable<WeekDTO>>(ErrorCode.NotAuthorized);

            if (!user.WeekSchedule.Any())
                return new Response<IEnumerable<WeekDTO>>(Enumerable.Empty<WeekDTO>());

            return new Response<IEnumerable<WeekDTO>>(user.WeekSchedule.Select(w => new WeekDTO(w) {
                Days = null
            }));
        }


        /// <summary>
        /// Gets list of <see cref="WeekNameDTO"/> for all schedules belonging to the user with the provided id
        /// </summary>
        /// <returns>List of <see cref="WeekNameDTO"/> on success else UserNotFound</returns>
        /// <param name="userId">User identifier for the <see cref="GirafUser" to get schedules for/></param>
        [HttpGet("v1/User/{userId}/week")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekNameDTO>>> ReadWeekSchedules(string userId)
        {
            var user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return new ErrorResponse<IEnumerable<WeekNameDTO>>(ErrorCode.UserNotFound);

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<IEnumerable<WeekNameDTO>>(ErrorCode.NotAuthorized);

            if (!user.WeekSchedule.Any())
                return new Response<IEnumerable<WeekNameDTO>>(Enumerable.Empty<WeekNameDTO>());

            return new Response<IEnumerable<WeekNameDTO>>(user.WeekSchedule.Select(w => new WeekNameDTO(w.WeekYear, w.WeekNumber, w.Name)));
        }

        /// <summary>
        /// Gets the <see cref="WeekDTO"/> with the specified week number and year for the user with the given id
        /// </summary>
        /// <param name="weekYear">The year of the week schedule to fetch.</param>
        /// <param name="weekNumber">The week number of the week schedule to fetch.</param>
        /// <returns><see cref="WeekDTO"/> for the requested week on success else UserNotFound or NotAuthorized</returns>
        /// <param name="userId">Identifier of the <see cref="GirafUser"/> to request schedule for</param>
        [HttpGet("v1/User/{userId}/week/{weekYear}/{weekNumber}")]
        [Authorize]
        public async Task<Response<WeekDTO>> ReadUsersWeekSchedule(string userId, int weekYear, int weekNumber)
        {
            var user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);

            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<WeekDTO>(ErrorCode.NotAuthorized);

            var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week != null)
            {
                foreach (var weekday in week.Weekdays)
                {
                    foreach (var activity in weekday.Activities)
                    {
                        if (activity.TimerKey != null)
                        {
                            var timerPlace = _giraf._context.Timers.FirstOrDefault(t => t.Key == activity.TimerKey);
                            activity.Timer = timerPlace;
                        }
                    }
                }

                return new Response<WeekDTO>(new WeekDTO(week));
            }

            //Create default thumbnail
            var emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");
            if (emptyThumbnail == null)
            {
                //Create default thumbnail
                _giraf._context.Pictograms.Add(new Pictogram("default", AccessLevel.PUBLIC));
                await _giraf._context.SaveChangesAsync();
                emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");

                return new Response<WeekDTO>(new WeekDTO() { WeekYear = weekYear, Name = $"{weekYear} - {weekNumber}", WeekNumber = weekNumber, Thumbnail = new Models.DTOs.WeekPictogramDTO(emptyThumbnail), Days = new int[] { 1, 2, 3, 4, 5, 6, 7 }.Select(d => new WeekdayDTO() { Activities = new List<ActivityDTO>(), Day = (Days)d }).ToArray() });
            }
            emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");

            return new Response<WeekDTO>(new WeekDTO()
            {
                WeekYear = weekYear,
                Name = $"{weekYear} - {weekNumber}",
                WeekNumber = weekNumber,
                Thumbnail = new Models.DTOs.WeekPictogramDTO(emptyThumbnail),
                Days = new[] { 1, 2, 3, 4, 5, 6, 7 }
                    .Select(d => new WeekdayDTO() { Activities = new List<ActivityDTO>(), Day = (Days)d }).ToArray()
            });

        }

        /// <summary>
        /// Updates the entire information of the week with the given year and week number.
        /// </summary>
        /// <param name="userId">id of user you want to get weekschedule for.</param>
        /// <param name="weekYear">year of the week you want to update</param>
        /// <param name="weekNumber">weeknr of week you want to update.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns><see cref="WeekDTO"/> for the requested week on success else MissingProperties, UserNotFound
        /// or NotAuthorized</returns>
        [HttpPut("v1/User/{userId}/week/{weekYear}/{weekNumber}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response<WeekDTO>> UpdateWeek(string userId, int weekYear, int weekNumber, [FromBody]WeekDTO newWeek)
        {
            if (newWeek == null) return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties);

            var user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null) return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);

            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse<WeekDTO>(ErrorCode.NotAuthorized);

            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week == null)
            {
                week = new Week() { WeekYear = weekYear, WeekNumber = weekNumber };
                user.WeekSchedule.Add(week);
            }

            var errorCode = await SetWeekFromDTO(newWeek, week, _giraf);
            if (errorCode != null)
                return new ErrorResponse<WeekDTO>(errorCode.ErrorCode, errorCode.ErrorProperties);

            _giraf._context.Weeks.Update(week);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekDTO>(new WeekDTO(week));
        }

        /// <summary>
        /// Deletes all information for the entire week with the given year and week number.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="weekYear"></param>
        /// <param name="weekNumber"></param>
        /// <returns>Success Reponse else UserNotFound, NotAuthorized,
        /// or NoWeekScheduleFound </returns>
        [HttpDelete("v1/User/{userId}/week/{weekYear}/{weekNumber}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        public async Task<Response> DeleteWeek(string userId, int weekYear, int weekNumber)
        {
            var user =  _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null) return new ErrorResponse(ErrorCode.UserNotFound);
            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return new ErrorResponse(ErrorCode.NotAuthorized);

            if (user.WeekSchedule.Any(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber))
            {
                var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
                if (week == null) return new ErrorResponse(ErrorCode.NoWeekScheduleFound);
                user.WeekSchedule.Remove(week);
                await _giraf._context.SaveChangesAsync();
                return new Response();
            }
            else
                return new ErrorResponse(ErrorCode.NoWeekScheduleFound);
        }
    }
}
