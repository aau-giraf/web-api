using System.Collections.Generic;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.IRepositories;

namespace GirafRest.Controllers
{
    /// <summary>
    /// Manages activities
    /// </summary>
    [Authorize]
    [Route("v2/[controller]")]
    public class ActivityController : Controller
    {
        private readonly IGirafUserRepository _userRepository;
        private readonly IAlternateNameRepository _alternateNameRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IWeekdayRepository _weekdayRepository;
        private readonly IPictogramRepository _pictogramRepository;
        private readonly IPictogramRelationRepository _pictogramRelationRepository;
        private readonly ITimerRepository _timerRepository;

        /// <summary>
        /// A data-logger used to write messages to the console. Handled by asp.net's dependency injection.
        /// </summary>
        public ILogger _logger { get; set; }

        /// <summary>
        /// Constructor for Controller
        /// </summary>
        /// <param name="loggerFactory">Service Injection</param>
        /// <param name="userRepository">Service Injection</param>
        /// <param name="alternateNameRepository">Service Injection</param>
        /// <param name="activityRepository">Service Injection</param>
        /// <param name="weekdayRepository">Service Injection</param>
        /// <param name="pictogramRepository">Service Injection</param>
        /// <param name="pictogramRelationRepository">Service Injection</param>
        /// <param name="timerRepository">Service Injection</param>
        public ActivityController(
            ILoggerFactory loggerFactory,
            IGirafUserRepository userRepository,
            IAlternateNameRepository alternateNameRepository,
            IActivityRepository activityRepository,
            IWeekdayRepository weekdayRepository,
            IPictogramRepository pictogramRepository,
            IPictogramRelationRepository pictogramRelationRepository,
            ITimerRepository timerRepository)
        {
            _logger = loggerFactory.CreateLogger("Activity");
            _userRepository = userRepository;
            _alternateNameRepository = alternateNameRepository;
            _activityRepository = activityRepository;
            _weekdayRepository = weekdayRepository;
            _pictogramRepository = pictogramRepository;
            _pictogramRelationRepository = pictogramRelationRepository;
            _timerRepository = timerRepository;
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

            GirafUser user = _userRepository.GetWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "Missing user"));

            var dbWeek = user.WeekSchedule.FirstOrDefault(w => w.WeekYear == weekYear && w.WeekNumber == weekNumber && string.Equals(w.Name, weekplanName));
            if (dbWeek == null)
                return NotFound(new ErrorResponse(ErrorCode.WeekNotFound, "Week not found"));

            Weekday dbWeekDay = dbWeek.Weekdays.FirstOrDefault(day => day.Day == weekDay);
            if (dbWeekDay == null)
                return NotFound(new ErrorResponse(ErrorCode.InvalidDay, "Day not found"));

            int order = dbWeekDay.Activities.Select(act => act.Order).DefaultIfEmpty(0).Max();
            order++;

            AlternateName alternateName = _alternateNameRepository.SingleOrDefault(alternateName
                => alternateName.Citizen == user
                && alternateName.PictogramId == newActivity.Pictograms.First().Id);

            string title = alternateName == null ? newActivity.Pictograms.First().Title : alternateName.Name;
            
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
            
            _activityRepository.Add(dbActivity);
            _weekdayRepository.Update(dbWeekDay);

            foreach (var pictogram in newActivity.Pictograms)
            {
                var dbPictogram = _pictogramRepository.Get(pictogram.Id);
                if (dbPictogram != null && string.IsNullOrEmpty(dbPictogram.Title))
                {
                    return BadRequest(new ErrorResponse(ErrorCode.InvalidProperties, "Invalid pictogram: Blank title"));
                }
                if (dbPictogram != null && !string.IsNullOrEmpty(dbPictogram.Title))
                {
                    _pictogramRelationRepository.Add(new PictogramRelation(
                        dbActivity, dbPictogram
                    ));
                }               
                else
                {
                    return NotFound(new ErrorResponse(ErrorCode.PictogramNotFound, "Pictogram not found"));
                }
            }

            // Unsure if we should save from every used repository, or just one of them.
            _userRepository.Save();

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
            GirafUser user = _userRepository.GetWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // throws error if none of user's weeks' has the specific activity
            if (!user.WeekSchedule.Any(w => w.Weekdays.Any(wd => wd.Activities.Any(act => act.Key == activityId))))
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            Activity targetActivity = _activityRepository.Get(activityId);

            // deletion of pictogram relations
            var pictogramRelations = _pictogramRelationRepository.Find(relation => relation.ActivityId == targetActivity.Key);

            _pictogramRelationRepository.RemoveRange(pictogramRelations);
            _activityRepository.Remove(targetActivity);

            // Unsure if we should save from every used repository, or just one of them.
            _userRepository.Save();

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
        public async Task<ActionResult> GetActivity(string userId, long activityId)
        {
            var activity = _activityRepository.Get(activityId);
            var pictograms = _pictogramRelationRepository.GetWithPictogram(activityId);

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

            GirafUser user = _userRepository.GetWithWeekSchedules(userId);
            if (user == null)
                return NotFound(new ErrorResponse(ErrorCode.UserNotFound, "User not found"));

            // throws error if none of user's weeks' has the specific activity
            if (!user.WeekSchedule.Any(w => w.Weekdays.Any(wd => wd.Activities.Any(act => act.Key == activity.Id))))
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            Activity updateActivity = _activityRepository.Get(activity.Id);
            if (updateActivity == null)
                return NotFound(new ErrorResponse(ErrorCode.ActivityNotFound, "Activity not found"));

            updateActivity.Order = activity.Order;
            updateActivity.State = activity.State;
            updateActivity.IsChoiceBoard = activity.IsChoiceBoard;
            updateActivity.ChoiceBoardName = activity.ChoiceBoardName;
            updateActivity.Title = activity.Title;

            // deletion of pictogram relations
            IEnumerable<PictogramRelation> pictogramRelations = _pictogramRelationRepository.Find(relation => relation.ActivityId == activity.Id);
            _pictogramRelationRepository.RemoveRange(pictogramRelations);

            List<WeekPictogramDTO> pictograms = new List<WeekPictogramDTO>();

            foreach (var pictogram in activity.Pictograms)
            {
                Pictogram db_pictogram = _pictogramRepository.Get(pictogram.Id);

                if (db_pictogram != null)
                {
                    _pictogramRelationRepository.Add(new PictogramRelation(
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
                Timer placeTimer = _timerRepository.Get(updateActivity.TimerKey);

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
                    Timer placeTimer = _timerRepository.Get(updateActivity.TimerKey);
                    if (placeTimer != null)
                    {
                        _timerRepository.Remove(placeTimer);
                    }
                    updateActivity.TimerKey = null;
                }
            }

            // Unsure if we should save from every used repository, or just one of them.
            _userRepository.Save();
            /*_activityRepository.Save();
            _pictogramRelationRepository.Save();
            _pictogramRepository.Save();
            _timerRepository.Save();*/

            return Ok(new SuccessResponse<ActivityDTO>(new ActivityDTO(updateActivity, pictograms)));
        }
    }
}
