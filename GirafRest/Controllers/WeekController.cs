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
using GirafRest.Models.Responses;
using static GirafRest.Controllers.SharedMethods;

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
        /// Gets all week schedule name and ids containing activities for the currently authenticated citizen.
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
                return new Response<WeekDTO>(new WeekDTO(week));
            
            //Create default thumbnail
            var emptyThumbnail = _giraf._context.Pictograms.FirstOrDefault(r => r.Title == "default");
            if (emptyThumbnail == null)
            {
                _giraf._context.Pictograms.Add(new Pictogram("default", AccessLevel.PUBLIC));
                await _giraf._context.SaveChangesAsync();
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
        /// <param name="id">If of the week to update information for.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns>NotFound if the user does not have a week schedule or if there exists no week with the given id,
        /// Ok and a serialized version of the updated week if everything went well.
        /// BadRequest if the body of the request does not contain a Week</returns>
        [HttpPut("{weekYear}/{weekNumber}")]
        [Authorize]
        public async Task<Response<WeekDTO>> UpdateWeek(int weekYear, int weekNumber, [FromBody]WeekDTO newWeek)
        {
            if (newWeek == null) return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties);
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<WeekDTO>(ErrorCode.UserNotFound);
            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
            if (week == null)
            {
                week = new Week() { WeekYear = weekYear, WeekNumber = weekNumber };
                user.WeekSchedule.Add(week);
            }
            if (newWeek.Thumbnail != null)
            {
                var thumbnail = await _giraf._context.Pictograms.Where(p => p.Id == newWeek.Thumbnail.Id).FirstOrDefaultAsync();
                if (thumbnail == null)
                    return new ErrorResponse<WeekDTO>(ErrorCode.ThumbnailDoesNotExist);
                week.Thumbnail = thumbnail;
            }
            else
            {
                return new ErrorResponse<WeekDTO>(ErrorCode.MissingProperties, "thumbnail");
            }
            
            week.Name = newWeek.Name;
            
            var errorCode = await UpdateWeekActivities(newWeek, week, _giraf);
            if (errorCode != ErrorCode.NoError)
                return new ErrorResponse<WeekDTO>(errorCode);

            _giraf._context.Weeks.Update(week);
            await _giraf._context.SaveChangesAsync();
            return new Response<WeekDTO>(new WeekDTO(week));
        }

        /// <summary>
        /// Deletes all information for the entire week with the given year and week number.
        /// </summary>
        /// <param name="id">If of the week to update information for.</param>
        /// <param name="newWeek">A serialized Week with new information.</param>
        /// <returns>NotFound if the user does not have a week schedule or
        /// Ok and a serialized version of the updated week if everything went well.</returns>
        [HttpDelete("{weekYear}/{weekNumber}")]
        [Authorize]
        public async Task<Response<IEnumerable<WeekBaseDTO>>> DeleteWeek(int weekYear, int weekNumber)
        {
            var user = await _giraf.LoadUserAsync(HttpContext.User);
            if (user == null) return new ErrorResponse<IEnumerable<WeekBaseDTO>>(ErrorCode.UserNotFound);
            if (user.WeekSchedule.Any(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber))
            {
                var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
                if (week == null) return new ErrorResponse<IEnumerable<WeekBaseDTO>>(ErrorCode.NoError);
                user.WeekSchedule.Remove(week);
                await _giraf._context.SaveChangesAsync();
                return new Response<IEnumerable<WeekBaseDTO>>(user.WeekSchedule.Select(w => new WeekDTO(w)));
            }
            else
                return new ErrorResponse<IEnumerable<WeekBaseDTO>>(ErrorCode.NoWeekScheduleFound);
        }
    }
}
