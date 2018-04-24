using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GirafRest.Services;
using System;
using GirafRest.Models.Responses;

namespace GirafRest.Controllers
{
    /// <summary>
    /// The WeekController allows the user to view and update his week schedule along with deleting it.
    /// </summary>
    [Route("v1/[controller]")]
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
        /// Gets all week schedule name and ids for the currently authenticated citizen.
        /// </summary>
        /// All WeekScheduleNameDTOs if succesfull request
        /// ErrorCode.UserNotFound if we cannot find any user in the DB
        /// ErrorCode.NoWeekScheduleFound if we can not find any weekschedule on the user
        /// <returns>
        /// </returns>
        [HttpGet("")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekNameDTO>>> ReadWeekSchedules()
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null)
            {
                return new ErrorResponse<IEnumerable<WeekNameDTO>>(ErrorCode.UserNotFound);
            }
            if (!user.WeekSchedule.Any())
            {
                return new ErrorResponse<IEnumerable<WeekNameDTO>>(ErrorCode.NoWeekScheduleFound);
            }
            return new Response<IEnumerable<WeekNameDTO>>(user.WeekSchedule.Select(w => new WeekNameDTO(w.WeekYear, w.WeekNumber, w.Name)));
        }

        /// <summary>
        /// Gets the schedule with the specified week number and year.
        /// </summary>
        /// <param name="weekYear">The year of the week schedule to fetch.</param>
        /// <param name="weekNumber">The week number of the week schedule to fetch.</param>
        /// <returns>NotFound if the user does not have a week with the given id or
        /// Ok and a serialized version of the week if he does.</returns>
        [HttpGet("{weekYear}/{weekNumber}")]
        [Authorize]
        public async Task<Response<WeekDTO>> ReadUsersWeekSchedule(int weekYear, int weekNumber) // changing this from id to year+week
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
            if (week != null)
            {
                return new Response<WeekDTO>(new WeekDTO(week));
            }
            else
            {
                return new Response<WeekDTO>(new WeekDTO() { WeekYear = weekYear, WeekNumber = weekNumber, Days = new int[] { 0, 1, 2, 3, 4, 5, 6 }.Select(d => new WeekdayDTO() { Activities = new List<ActivityDTO>(), Day = (Days)d }).ToArray() });
            }
        }

        /// <summary>
        /// Updates the entire information of the week with the given id.
        /// </summary>
        /// <param name="id">If of the week to update information for.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns>NotFound if the user does not have a week schedule or if there exists no week with the given id,
        /// Ok and a serialized version of the updated week if everything went well.
        /// BadRequest if the body of the request does not contain a Week</returns>
        [HttpPut("{weekYear}/{weekNumber}")]
        [Authorize]
        public async Task<Response<WeekDTO>> UpdateWeek(int weekYear, int weekNumber, [FromBody]WeekDTO newWeek)
        {
            //return Ok(newWeek);
            if (newWeek == null) return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties);
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);
            var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
            if (week == null)
            {
                week = new Week();
            }
            if (newWeek.Thumbnail != null)
            {
                var thumbnail = await _giraf._context.Pictograms.Where(p => p.Id == newWeek.Thumbnail.Id).FirstOrDefaultAsync();
                if (thumbnail == null)
                    return new ErrorResponse<WeekDTO>(ErrorCode.ThumbnailDoesNotExist);
                week.Thumbnail = thumbnail;
            }
            week.Name = newWeek.Name;
            var modelErrorCode = newWeek.ValidateModel();
            if (modelErrorCode.HasValue)
                return new ErrorResponse<WeekDTO>(modelErrorCode.Value, "Week should contain at least 1 day and no more than 7 days.");
            var orderedDays = week.Weekdays.OrderBy(w => w.Day).ToArray();
            foreach (var day in newWeek.Days)
            {
                var wkDay = new Weekday(day);
                if (!(await CreateWeekDayHelper(wkDay, day)))
                    return new ErrorResponse<WeekDTO>(ErrorCode.ResourceNotFound);
                week.UpdateDay(wkDay);
            }
            _giraf._context.Weeks.Update(week);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekDTO>(new WeekDTO(week));
        }
    
        /// <summary>
        /// Deletes the entire week with the given id.
        /// </summary>
        /// <param name="id">Id of the week to delete.</param>
        /// <returns>NotFound if the user does not have a week schedule or
        /// Ok and a serialized version of the updated week if everything went well.</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekDTO>>> DeleteWeek(long id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<IEnumerable<WeekDTO>>(ErrorCode.UserNotFound);
            if (user.WeekSchedule.Any(w => w.Id == id))
            {
                var week = user.WeekSchedule.FirstOrDefault(w => w.Id == id);
                if (week == null) return new ErrorResponse<IEnumerable<WeekDTO>>(ErrorCode.WeekScheduleNotFound);
                user.WeekSchedule.Remove(week);
                await _giraf._context.SaveChangesAsync();
                return new Response<IEnumerable<WeekDTO>>(user.WeekSchedule.Select(w => new WeekDTO(w)));
            }
            else
                return new ErrorResponse<IEnumerable<WeekDTO>>(ErrorCode.NoWeekScheduleFound);
        }

        #region helpers

        /// <summary>
        /// Take pictograms and choices from DTO and add them to weekday object.
        /// </summary>
        /// <returns>True if all pictograms and choices were found and added, and false otherwise.</returns>
        /// <param name="to">Pictograms and choices will be added to this object.</param>
        /// <param name="from">Pictograms and choices will be read from this object.</param>
        private async Task<bool> CreateWeekDayHelper(Weekday to, WeekdayDTO from){
            foreach (var elem in from.Activities)
            {
                var picto = await _giraf._context.Pictograms.Where(p => p.Id == elem.Pictogram.Id).FirstOrDefaultAsync();
                if (picto != null)
                    to.Activities.Add(new Activity(to, picto, elem.Order));
            }
            return true;
        }


        #endregion
    }
}
