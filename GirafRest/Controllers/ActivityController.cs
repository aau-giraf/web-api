using System.Collections.Generic;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages activities
    /// </summary>
    [Authorize]
    [Route("v2/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IAuthenticationService _authentication;
        private readonly IGirafService _giraf;

        /// <summary>
        /// Constructor for Controller
        /// </summary>
        /// <param name="giraf">Service Injection</param>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="authentication">Service Injection</param>
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
        /// <param name="weekDayNmb">day of the week that you want to add the activity on (Monday=1, Sunday=7).</param>
        /// <returns>Returns <see cref="ActivityDTO"/> for the requested activity on success else MissingProperties, 
        /// UserNotFound, NotAuthorized, WeekNotFound or InvalidDay.</returns>
        [HttpPost("{userId}/{weekplanName}/{weekYear}/{weekNumber}/{weekDayNmb}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PostActivity([FromBody] ActivityDTO newActivity, string userId, string weekplanName, int weekYear, int weekNumber, int weekDayNmb)
        {
            Days weekDay = (Days)weekDayNmb;
            if (newActivity == null)
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing new activity"));

            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "Missing user"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            var dbWeek = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber && string.Equals(w.Name, weekplanName));
            if (dbWeek == null)
                return NotFound(new ErrorResponse(ErrorCode.WeekNotFound, "Week not found"));

            Weekday dbWeekDay = dbWeek.Weekdays.FirstOrDefault(day => day.Day == weekDay);
            if (dbWeekDay == null)
                return NotFound(new ErrorResponse(ErrorCode.InvalidDay, "Day not found"));

            int order = dbWeekDay.Activities.Select(act => act.Order).DefaultIfEmpty(0).Max();
            order++;


            AlternateName an = await _giraf._context.AlternateNames.FirstOrDefaultAsync(altnam =>
                altnam.Citizen == user && altnam.PictogramId == newActivity.Pictograms.First().Id);

            string title = an == null ? newActivity.Pictograms.First().Title : an.Name;
            
            Activity dbActivity = new Activity(
                dbWeekDay,
                null,
                order,
                ActivityState.Normal,
                null,
                false,
                title
            );
            dbWeekDay.Activities.Add(dbActivity);
            _giraf._context.Activities.Add(dbActivity);
            _giraf._context.Weekdays.Update(dbWeekDay);

            foreach (var pictogram in newActivity.Pictograms)
            {
                var dbPictogram = _giraf._context.Pictograms.FirstOrDefault(id => id.Id == pictogram.Id);
                if (dbPictogram != null && string.IsNullOrEmpty(dbPictogram.Title))
                {
                    return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Invalid pictogram: Blank title"));
                }
                if (dbPictogram != null && !string.IsNullOrEmpty(dbPictogram.Title))
                {
                    _giraf._context.PictogramRelations.Add(new PictogramRelation(
                        dbActivity, dbPictogram
                    ));
                }               
                else
                {
                    return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
                }
            }

            await _giraf._context.SaveChangesAsync();

            return StatusCode(
                StatusCodes.Status201Created,
                new SuccessResponse<ActivityDTO>(
                    new ActivityDTO(dbActivity, newActivity.Pictograms.ToList())
                )
            );
        }

        /// <summary>
        /// Delete an activity with a given id.
        /// </summary>
        /// <param name="userId">id of the user you want to delete an activity for.</param>
        /// <param name="activityId">id of the activity you want to delete.</param>
        /// <returns>Returns success response else UserNotFound, NotAuthorized or ActivityNotFound.</returns>
        [HttpDelete("{userId}/delete/{activityId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteActivity(string userId, long activityId)
        {
            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rights
            if (!(await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user)))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permission"));

            // throws error if none of user's weeks' has the specific activity
            if (!user.WeekSchedule.Any(w => w.Weekdays.Any(wd => wd.Activities.Any(act => act.Key == activityId))))
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            Activity targetActivity = _giraf._context.Activities.First(act => act.Key == activityId);

            // deletion of pictogram relations
            var pictogramRelations = _giraf._context.PictogramRelations
                .Where(relation => relation.ActivityId == targetActivity.Key);

            _giraf._context.PictogramRelations.RemoveRange(pictogramRelations);

            _giraf._context.Activities.Remove(targetActivity);
            await _giraf._context.SaveChangesAsync();

            return Ok(new SuccessResponse("Activity deleted"));
        }

        /// <summary>
        /// Gets a user's activity from an activity id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="activityId"></param>
        /// <returns>Returns <see cref="ActivityDTO"/></returns>
        [HttpGet("{userId}/{activityId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetActivity(string userId, int activityId)
        {
            var activity = await _giraf._context
                .Activities
                .Where(a => a.Key == activityId)
                .FirstOrDefaultAsync();

            var pictograms = _giraf._context
                .PictogramRelations
                .Include(pictogram => pictogram.Pictogram)
                .Where(pr => pr.ActivityId == activityId)
                .ToList();

            activity.Pictograms = pictograms;

            return Ok(new SuccessResponse<ActivityDTO>(new ActivityDTO(activity)));
        }

        /// <summary>
        /// Updates an activity with a given id.
        /// </summary>
        /// <param name="activity">a serialized version of the activity that will be updated.</param>
        /// <param name="userId">an ID of the user to update activities for.</param>
        /// <returns>Returns <see cref="ActivityDTO"/> for the updated activity on success else MissingProperties or NotFound</returns>
        [HttpPut("{userId}/update")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateActivity([FromBody] ActivityDTO activity, string userId)
        {
            if (activity == null)
            {
                return BadRequest(new ErrorResponse(ErrorCode.MissingProperties, "Missing activity"));
            }

            GirafUser user = await _giraf.LoadUserWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // check access rights
            if (!await _authentication.HasEditOrReadUserAccess(await _giraf._userManager.GetUserAsync(HttpContext.User), user))
                return StatusCode(StatusCodes.Status403Forbidden, new ErrorResponse(ErrorCode.NotAuthorized, "User does not have permissions"));

            // throws error if none of user's weeks' has the specific activity
            if (!user.WeekSchedule.Any(w => w.Weekdays.Any(wd => wd.Activities.Any(act => act.Key == activity.Id))))
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            Activity updateActivity = _giraf._context.Activities
                .FirstOrDefault(a => a.Key == activity.Id);
            if (updateActivity == null)
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            updateActivity.Order = activity.Order;
            updateActivity.State = activity.State;
            updateActivity.IsChoiceBoard = activity.IsChoiceBoard;
            updateActivity.ChoiceBoardName = activity.ChoiceBoardName;
            updateActivity.Title = activity.Title;

            // deletion of pictogram relations

            var pictogramRelations = _giraf._context.PictogramRelations
                .Where(relation => relation.ActivityId == activity.Id);

            _giraf._context.PictogramRelations.RemoveRange(pictogramRelations);

            List<WeekPictogramDTO> pictograms = new List<WeekPictogramDTO>();

            foreach (var pictogram in activity.Pictograms)
            {
                var db_pictogram = _giraf._context.Pictograms.Single(id => id.Id == pictogram.Id);

                if (db_pictogram != null)
                {
                    _giraf._context.PictogramRelations.Add(new PictogramRelation(
                        updateActivity, db_pictogram
                    ));
                    pictograms.Add(new WeekPictogramDTO(db_pictogram));
                }
                else
                {
                    return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
                }
            }

            if (activity.Timer != null)
            {
                Timer placeTimer = _giraf._context.Timers.FirstOrDefault(t => t.Key == updateActivity.TimerKey);

                if (updateActivity.TimerKey == null)
                {
                    updateActivity.TimerKey = activity.Timer.Key;
                }

                if (placeTimer != null)
                {
                    placeTimer.StartTime = activity.Timer.StartTime;
                    placeTimer.Progress = activity.Timer.Progress;
                    placeTimer.FullLength = activity.Timer.FullLength;
                    placeTimer.Paused = activity.Timer.Paused;

                    updateActivity.Timer = placeTimer;
                }
                else
                {
                    updateActivity.Timer = new Timer()
                    {
                        StartTime = activity.Timer.StartTime,
                        Progress = activity.Timer.Progress,
                        FullLength = activity.Timer.FullLength,
                        Paused = activity.Timer.Paused,
                    };
                }
            }
            else
            {
                if (updateActivity.TimerKey != null)
                {
                    Timer placeTimer = _giraf._context.Timers.FirstOrDefault(t => t.Key == updateActivity.TimerKey);
                    if (placeTimer != null)
                    {
                        _giraf._context.Timers.Remove(placeTimer);
                    }
                    updateActivity.TimerKey = null;
                }
            }

            await _giraf._context.SaveChangesAsync();

            return Ok(new SuccessResponse<ActivityDTO>(new ActivityDTO(updateActivity, pictograms)));
        }
    }
}
