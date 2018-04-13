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
            return new Response<IEnumerable<WeekNameDTO>>(user.WeekSchedule.Select(w => new WeekNameDTO(w.Id, w.Name)));
        }


        /// <summary>
        /// Gets the schedule with the specified id.
        /// </summary>
        /// <param name="id">The id of the week schedule to fetch.</param>
        /// <returns>NotFound if the user does not have a week with the given id or
        /// Ok and a serialized version of the week if he does.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<Response<WeekDTO>> ReadUsersWeekSchedule(long id)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            var week = user.WeekSchedule.FirstOrDefault(w => w.Id == id);
            if (week != null)
            {
                return new Response<WeekDTO>(new WeekDTO(week));
            }
            else
                return new ErrorResponse<WeekDTO>(ErrorCode.WeekScheduleNotFound);
        }

        /// <summary>
        /// Updates the entire information of the week with the given id.
        /// </summary>
        /// <param name="id">If of the week to update information for.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns>NotFound if the user does not have a week schedule or if there exists no week with the given id,
        /// Ok and a serialized version of the updated week if everything went well.
        /// BadRequest if the body of the request does not contain a Week</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<Response<WeekDTO>> UpdateWeek(long id, [FromBody]WeekDTO newWeek)
        {
            //return Ok(newWeek);
            if (newWeek == null) return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties);
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);
            var week = user.WeekSchedule.FirstOrDefault(w => w.Id == id);
            if (week == null)
                return new ErrorResponse<WeekDTO>(ErrorCode.WeekScheduleNotFound);
            if (newWeek.Thumbnail != null)
            {
                var thumbnail = await _giraf._context.Pictograms.Where(p => p.Id == newWeek.Thumbnail.Id).FirstOrDefaultAsync();
                if (thumbnail == null)
                    return new ErrorResponse<WeekDTO>(ErrorCode.ThumbnailDoesNotExist);
                week.Thumbnail = thumbnail;
            }
            week.Name = newWeek.Name;
            // If newWeek.Days should support number of days other than 7, change this check to if(newWeek.Days.Count < 1)
            if (newWeek.Days == null || newWeek.Days.Count != 7)
                return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties, "days");

            //If two days have the same day index
            if(newWeek.Days.GroupBy(d => d.Day).Any(g => g.Count() != 1))
                return new ErrorResponse<WeekDTO>(ErrorCode.TwoDaysCannotHaveSameDayProperty);

            var orderedDays = week.Weekdays.OrderBy(w => w.Day).ToArray();

            foreach (var day in newWeek.Days)
            {
                var wkDay = new Weekday(day) {LastEdit = DateTime.Now};
                if (!(await CreateWeekDayHelper(wkDay, day.ElementIDs)))
                        return new ErrorResponse<WeekDTO>(ErrorCode.ResourceNotFound);
                orderedDays[(int)day.Day].Elements = wkDay.Elements;
            }
            _giraf._context.Weeks.Update(week);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekDTO>(new WeekDTO(week));
        }

        /// <summary>
        /// Creates the week.
        /// </summary>
        /// <returns>
        /// Invalidproperties if newWeek has incorrect format or if the days does not lie in the int range from 0-6
        /// or if the day does not have exactly 7 days
        /// UserNotFound if no authenticated user
        /// RessourceNotFound if trying to add a ressource that does not exist
        /// The DTO corresponding to the created week if succesfull</returns>
        /// <param name="newWeek">New week.</param>
        [HttpPost("")]
        [Authorize]
        public async Task<Response<WeekDTO>> CreateWeek([FromBody]WeekDTO newWeek)
        {
            if (newWeek == null) 
                return new ErrorResponse<WeekDTO>(ErrorCode.InvalidProperties);
            if (newWeek.Days == null || newWeek.Days.Count != 7) 
                return new ErrorResponse<WeekDTO>(ErrorCode.InvalidProperties, "Week should contain no more and no less than 7 days.");
            
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) 
                return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);
            var thumbnail = await _giraf._context.Pictograms.Where(p => p.Id == newWeek.Thumbnail.Id).FirstOrDefaultAsync();
            
            if (thumbnail == null)
                return new ErrorResponse<WeekDTO>(ErrorCode.ThumbnailDoesNotExist);

            var week = new Week(thumbnail) {Name = newWeek.Name};
            if (newWeek.Days != null)
            {
                foreach (var day in newWeek.Days)
                {
                    if (!(Enum.IsDefined(typeof(Days), day.Day))) return new ErrorResponse<WeekDTO>(ErrorCode.InvalidProperties);
                    Weekday wkDay = week.Weekdays[(int)day.Day];
                    wkDay.LastEdit = DateTime.Now;
                    if(!(await CreateWeekDayHelper(wkDay, day.ElementIDs)))
                        return new ErrorResponse<WeekDTO>(ErrorCode.ResourceNotFound);

                    week.Weekdays[(int)day.Day].Elements = wkDay.Elements;
                }
            }
            _giraf._context.Weeks.Add(week);
            user.WeekSchedule.Add(week);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekDTO>(new WeekDTO(user.WeekSchedule.Last()));
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
        /// Helper for adding pictograms to a weekday
        /// </summary>
        /// <returns>The week day helper.</returns>
        /// <param name="wkDay">Wk day.</param>
        /// <param name="Ids">Identifiers.</param>
        private async Task<bool> CreateWeekDayHelper(Weekday wkDay, List<long> Ids){
            foreach (var elemId in Ids)
            {
                var picto = await _giraf._context.Frames.Where(p => p.Id == elemId).FirstOrDefaultAsync();
                if (picto != null)
                    wkDay.Elements.Add(new WeekdayResource(wkDay, picto));
                else
                {
                    var choice = await _giraf._context.Choices.Where(c => c.Id == elemId).FirstOrDefaultAsync();
                    if (choice != null)
                        wkDay.Elements.Add(new WeekdayResource(wkDay, choice));
                    else
                        return false;
                }
            }
            return true;
        }


        #endregion
    }
}
