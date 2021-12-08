using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static GirafRest.Shared.SharedMethods;
using GirafRest.IRepositories;


namespace GirafRest.Controllers
{
    /// <summary>
    /// Controller for managing Weeks, CRUD-ish
    /// </summary>
    [Authorize]
    [Route("v1/[controller]")]
    public class WeekController : Controller
    {
        private readonly IGirafService _giraf;


        private readonly IWeekRepository _weekRepository;
        private readonly ITimerRepository _timerRepository;
        private readonly IPictogramRepository _pictogramRepository;
        private readonly IWeekdayRepository _weekdayRepository;

        /// <summary>
        /// Constructor for WeekController
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="weekRepository">Service Injection</param>
        /// <param name="timerRepository">Service Injection</param>
        /// <param name="pictogramRepository">Service Injection</param>
        /// <param name="weekdayRepository">Service Injection</param>
        public WeekController(IGirafService giraf, ILoggerFactory loggerFactory, IWeekRepository weekRepository, ITimerRepository timerRepository, IPictogramRepository pictogramRepository, IWeekdayRepository weekdayRepository)
        {
            _giraf = giraf;
            _giraf._logger = loggerFactory.CreateLogger("Week");
            _weekRepository = weekRepository;
            _timerRepository = timerRepository;
            _pictogramRepository = pictogramRepository;
            _weekdayRepository = weekdayRepository;
        }

        /// <summary>
        /// Gets list of <see cref="WeekDTO"/> for all weeks belonging to the user with the provided id, days not are included
        /// </summary>
        /// <returns>List of <see cref="WeekDTO"/> on success else UserNotFound</returns>
        /// <param name="userId">User identifier for the <see cref="GirafUser" /> to get schedules for/></param>
        /// refactored to repository
        [HttpGet("{userId}/week", Name = "GetListOfWeeksExclDaysOfUser")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<WeekDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadFullWeekSchedules(string userId)
        {
            var user = await _weekRepository.getAllWeeksOfUser(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
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
        /// refactored to repository
        [HttpGet("{userId}/weekName", Name = "GetListOfWeekNamesOfUser")]
        [ProducesResponseType(typeof(SuccessResponse<IEnumerable<WeekNameDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadWeekSchedules(string userId)
        {
            var user = await _weekRepository.getAllWeeksOfUser(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            if (!user.WeekSchedule.Any())
                return Ok(new SuccessResponse<IEnumerable<WeekNameDTO>>(Enumerable.Empty<WeekNameDTO>()));
            // Sort Returnlist 
            List<WeekNameDTO> returnlist = user.WeekSchedule.Select(w => new WeekNameDTO(w.WeekYear, w.WeekNumber, w.Name)).ToList();
            returnlist.Sort();

            return Ok(new SuccessResponse<IEnumerable<WeekNameDTO>>(returnlist));
        }

        /// <summary>
        /// Gets the <see cref="WeekDTO"/> with the specified week number and year for the user with the given id
        /// </summary>
        /// <param name="weekYear">The year of the week schedule to fetch.</param>
        /// <param name="weekNumber">The week number of the week schedule to fetch.</param>
        /// <returns><see cref="WeekDTO"/> for the requested week on success else UserNotFound or NotAuthorized</returns>
        /// <param name="userId">Identifier of the <see cref="GirafUser"/> to request schedule for</param>
        /// refactored to repository
        [HttpGet("{userId}/{weekYear}/{weekNumber}", Name = "GetWeekByWeekNrAndYearOfUser")]
        [ProducesResponseType(typeof(SuccessResponse<WeekDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ReadUsersWeekSchedule(string userId, int weekYear, int weekNumber)
        {
            var user = await _weekRepository.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week != null)
            {
                foreach (var weekday in week.Weekdays)
                {
                    foreach (var activity in weekday.Activities)
                    {
                        if (activity.TimerKey != null)
                        {
                            activity.Timer = await _timerRepository.getActivitysTimerkey(activity);
                            
                        }

                        if (activity.Pictograms != null)
                        {
                            foreach (var pictogramRelation in activity.Pictograms)
                            {
                                var dbPictogram = await _pictogramRepository.getPictogramMatchingRelation(pictogramRelation);
                                if (dbPictogram != null)
                                {
                                    pictogramRelation.Pictogram = dbPictogram;
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
            var emptyThumbnail = await _pictogramRepository.GetPictogramWithName("default");
            if (emptyThumbnail == null)
            {
                //Create default thumbnail
                await _pictogramRepository.AddPictogramWith_NO_ImageHash("default", AccessLevel.PUBLIC);
                
                emptyThumbnail = await _pictogramRepository.GetPictogramWithName("default");

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
        /// Gets the <see cref="WeekdayDTO"/> with the specified week number and year for the user with the given id
        /// </summary>
        /// <param name="userId">Identifier of the <see cref="GirafUser"/> to request schedule for</param>
        /// <param name="weekYear">The year of the week schedule to fetch.</param>
        /// <param name="weekNumber">The week number of the week schedule to fetch.</param>
        /// <param name="day">The index of the day of the week. (Monday = 1 and sunday = 7)</param>
        /// <returns><see cref="WeekdayDTO"/> for the requested week on success else InvalidDay, UserNotFound, 
        /// NotAuthorized or NotFound</returns>
        /// refactored to repository
        [HttpGet("{userId}/{weekYear}/{weekNumber}/{day}")]
        [ProducesResponseType(typeof(SuccessResponse<WeekdayDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetWeekDay(string userId, int weekYear, int weekNumber, int day)
        {
            if (day < 0 || day > 6)
            {
                return BadRequest(new ErrorResponse(ErrorCode.InvalidDay, "Day must be between 0 and 6"));
            }
            
            var user = await _weekRepository.LoadUserWithWeekSchedules(userId);
            if (user == null) return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
 
            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.NotFound, "Week not found"));
            }

            Weekday weekday = week.Weekdays.SingleOrDefault(d => d.Day == (Days)day + 1);
            if (weekday == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.NotFound, "Weekday not found"));
            }

            foreach (var activity in weekday.Activities)
            {
                if (activity.TimerKey != null)
                {
                    var timerPlace = await _timerRepository.getActivitysTimerkey(activity);
                    activity.Timer = timerPlace;
                }

                if (activity.Pictograms != null)
                {
                    foreach (var pictogramRelation in activity.Pictograms)
                    {
                        
                        var dbPictogram = await _pictogramRepository.getPictogramMatchingRelation(pictogramRelation);
                        if (dbPictogram != null)
                        {
                            pictogramRelation.Pictogram = dbPictogram;
                        }
                        else
                        {
                            return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound,
                                "Pictogram not found"));
                        }
                    }
                }
            }

            return Ok(new SuccessResponse<WeekdayDTO>(new WeekdayDTO(weekday)));
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
        [HttpPut("{userId}/{weekYear}/{weekNumber}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse<WeekDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateWeek(string userId, int weekYear, int weekNumber, [FromBody] WeekDTO newWeek)
        {
            if (newWeek == null) return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing newWeek"));

            var user = await _weekRepository.LoadUserWithWeekSchedules(userId);
            if (user == null) return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week == null)
            {
                week = new Week() { WeekYear = weekYear, WeekNumber = weekNumber};
                user.WeekSchedule.Add(week);
            }

            var errorCode = await _weekRepository.SetWeekFromDTO(newWeek, week);
            if (errorCode != null)
                return BadRequest(errorCode);

            await _weekRepository.UpdateSpecificWeek(week);
            return Ok(new SuccessResponse<WeekDTO>(new WeekDTO(week)));
        }

        /// <summary>
        /// Updates the entire information of the <see cref="WeekdayDTO"/> with the given year, week number and
        /// the user id."/>.
        /// </summary>
        /// <param name="userId">id of user you want to get weekschedule for.</param>
        /// <param name="weekYear">year of the week you want to update</param>
        /// <param name="weekNumber">weeknumber of week you want to update.</param>
        /// <param name="weekdayDto">A serialized <see cref="Weekday"/> with the new information</param>
        /// <returns><see cref="WeekdayDTO"/> for the requested week on success else UserNotFound, MissingProperties,
        /// NotAuthorized or NotFound</returns>
        /// refactored to repository
        [HttpPut("day/{userId}/{weekYear}/{weekNumber}")]
        [ProducesResponseType(typeof(SuccessResponse<WeekdayDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateWeekday(string userId, int weekYear, int weekNumber,
            [FromBody] WeekdayDTO weekdayDto)
        {
            if (weekdayDto == null)
            {
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing weekday"));
            }
            
            var user = await _weekRepository.LoadUserWithWeekSchedules(userId);
            if (user == null) return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));
            
            
            Week week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);

            if (week == null)
            {
                return NotFound(new ErrorResponse(ErrorCode.WeekNotFound, "Week not found"));
            }

            Weekday oldDay = week.Weekdays.Single(d => d.Day == weekdayDto.Day);

            oldDay.Activities.Clear();
            if (!await _weekRepository.AddPictogramsToWeekday(oldDay, weekdayDto))
            {
                return NotFound(new ErrorResponse(ErrorCode.ResourceNotFound, "Missing pictogram"));
            }

            await _weekdayRepository.UpdateSpecificWeekDay(oldDay);


            return Ok(new SuccessResponse<WeekdayDTO>(new WeekdayDTO(oldDay)));
        }


        /// <summary>
        /// Deletes all information for the entire week with the given year and week number.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="weekYear"></param>
        /// <param name="weekNumber"></param>
        /// <returns>Success Reponse else UserNotFound, NotAuthorized,
        /// or NoWeekScheduleFound </returns>
        /// refactored to repository
        [HttpDelete("{userId}/{weekYear}/{weekNumber}")]
        [Authorize(Roles = GirafRole.Department + "," + GirafRole.Guardian + "," + GirafRole.SuperUser)]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteWeek(string userId, int weekYear, int weekNumber)
        {
            var user = await _weekRepository.getAllWeeksOfUser(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));


            var week = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber);
            if (week != null)
            {

                await _weekRepository.DeleteSpecificWeek(user, week);
                return Ok(new SuccessResponse("Deleted info for entire week"));
            }
            else
                return NotFound(new ErrorResponse(ErrorCode.NoWeekScheduleFound, "No week schedule found"));
        }
    }
}