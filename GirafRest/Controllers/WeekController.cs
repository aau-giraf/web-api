using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GirafRest.Shared.SharedMethods;

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
        /// <param name="userId">User identifier for the <see cref="GirafUser" /> to get schedules for/></param>
        [HttpGet("v2/User/{userId}/week", Name = "GetListOfWeeksExclDaysOfUser")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<WeekDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadFullWeekSchedules(string userId)
        {
            var user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, ""));

            if (!user.WeekSchedule.Any())
                return Ok(new SuccessResponse<IEnumerable<WeekDTO>>(Enumerable.Empty<WeekDTO>()));

            return Ok(new SuccessResponse<IEnumerable<WeekDTO>>(user.WeekSchedule.Select(w => new WeekDTO(w)
            {
                Days = null
            })));
        }


        /// <summary>
        /// Gets list of <see cref="WeekNameDTO"/> for all schedules belonging to the user with the provided id
        /// </summary>
        /// <returns>List of <see cref="WeekNameDTO"/> on success else UserNotFound</returns>
        /// <param name="userId">User identifier for the <see cref="GirafUser" /> to get schedules for</param>
        [HttpGet("v1/User/{userId}/week", Name = "GetListOfWeekNamesOfUser")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<WeekNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadWeekSchedules(string userId)
        {
            var user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            if (!user.WeekSchedule.Any())
                return Ok(new SuccessResponse<IEnumerable<WeekNameDTO>>(Enumerable.Empty<WeekNameDTO>()));

            return Ok(new SuccessResponse<IEnumerable<WeekNameDTO>>(user.WeekSchedule.Select(w => new WeekNameDTO(w.WeekYear, w.WeekNumber, w.Name))));
        }

        /// <summary>
        /// Gets the <see cref="WeekDTO"/> with the specified week number and year for the user with the given id
        /// </summary>
        /// <param name="weekYear">The year of the week schedule to fetch.</param>
        /// <param name="weekNumber">The week number of the week schedule to fetch.</param>
        /// <returns><see cref="WeekDTO"/> for the requested week on success else UserNotFound or NotAuthorized</returns>
        /// <param name="userId">Identifier of the <see cref="GirafUser"/> to request schedule for</param>
        [HttpGet("v1/User/{userId}/week/{weekYear}/{weekNumber}", Name = "GetWeekByWeekNrAndYearOfUser")]
        [Authorize]
        [ProducesResponseType(typeof(SuccessResponse<WeekDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadUsersWeekSchedule(string userId, int weekYear, int weekNumber)
        {
            var user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

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

                        if (activity.Pictograms != null)
                        {
                            foreach (var pictogramRelation in activity.Pictograms)
                            {
                                var dbPictogram =
                                    _giraf._context.Pictograms.FirstOrDefault(p => p.Id == pictogramRelation.PictogramId);
                                if (dbPictogram != null)
                                {
                                    pictogramRelation.Pictogram =
                                        _giraf._context.Pictograms.Single(p => p.Id == pictogramRelation.PictogramId);
                                }
                                else
                                {
                                    return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound,
                                        "Pictogram not found"));
                                }
                            }
                        }
                    }
                }

                return Ok(new SuccessResponse<WeekDTO>(new WeekDTO(week)));
            }

            //Create default thumbnail
            var emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");
            if (emptyThumbnail == null)
            {
                //Create default thumbnail
                _giraf._context.Pictograms.Add(new Pictogram("default", AccessLevel.PUBLIC));
                await _giraf._context.SaveChangesAsync();
                emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");

                return Ok(new SuccessResponse<WeekDTO>(new WeekDTO()
                {
                    WeekYear = weekYear,
                    Name = $"{weekYear} - {weekNumber}",
                    WeekNumber = weekNumber,
                    Thumbnail = new Models.DTOs.WeekPictogramDTO(emptyThumbnail),
                    Days = new int[] { 1, 2, 3, 4, 5, 6, 7 }
                        .Select(d => new WeekdayDTO()
                        {
                            Activities = new List<ActivityDTO>(),
                            Day = (Days)d
                        }).ToArray()
                }));
            }
            emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");

            return Ok(new SuccessResponse<WeekDTO>(new WeekDTO()
            {
                WeekYear = weekYear,
                Name = $"{weekYear} - {weekNumber}",
                WeekNumber = weekNumber,
                Thumbnail = new Models.DTOs.WeekPictogramDTO(emptyThumbnail),
                Days = new[] { 1, 2, 3, 4, 5, 6, 7 }
                    .Select(d => new WeekdayDTO()
                    {
                        Activities = new List<ActivityDTO>(),
                        Day = (Days)d
                    }).ToArray()
            }));
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
        [ProducesResponseType(typeof(SuccessResponse<WeekDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateWeek(string userId, int weekYear, int weekNumber, [FromBody]WeekDTO newWeek)
        {
            if (newWeek == null) return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing newWeek"));

            var user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null) return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week == null)
            {
                week = new Week() { WeekYear = weekYear, WeekNumber = weekNumber };
                user.WeekSchedule.Add(week);
            }

            var errorCode = await SetWeekFromDTO(newWeek, week, _giraf);
            if (errorCode != null)
                return BadRequest(errorCode);

            _giraf._context.Weeks.Update(week);
            await _giraf._context.SaveChangesAsync();
            return Ok(new SuccessResponse<WeekDTO>(new WeekDTO(week)));
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
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteWeek(string userId, int weekYear, int weekNumber)
        {
            var user = _giraf._context.Users.Include(u => u.WeekSchedule).FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rightss
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            if (user.WeekSchedule.Any(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber))
            {
                var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
                if (week == null)
                    return NotFound(new ErrorResponse(ErrorCode.NoWeekScheduleFound, "No week schedule found"));
                user.WeekSchedule.Remove(week);

                await _giraf._context.SaveChangesAsync();
                return Ok(new SuccessResponse("Deleted info for entire week"));
            }
            else
                return NotFound(new ErrorResponse(ErrorCode.NoWeekScheduleFound, "No week schedule found"));
        }
    }
}
