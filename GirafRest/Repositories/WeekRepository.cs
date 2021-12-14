using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using GirafRest.Models.Responses;
using GirafRest.Models.DTOs;

namespace GirafRest.Repositories
{
    public class WeekRepository : Repository<Week>, IWeekRepository
    {
        public WeekRepository(GirafDbContext context) : base(context)
        {
        }

        public Task<GirafUser> getAllWeeksOfUser(string userId) { 

            return Context.Users.Include(u => u.WeekSchedule).FirstOrDefaultAsync(u => u.Id == userId);
        }
        /// <summary>
        /// Method for loading user from context and eager loading fields required to read their <b>week schedules</b>
        /// </summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public async Task<GirafUser> LoadUserWithWeekSchedules(string id)
        {
            var user = await Context.Users
                //First load the user from the database
                .Where(u => u.Id.ToLower() == id.ToLower())
                // then load his week schedule
                .Include(u => u.WeekSchedule)
                .ThenInclude(w => w.Thumbnail)
                .Include(u => u.WeekSchedule)
                .ThenInclude(w => w.Weekdays)
                .ThenInclude(wd => wd.Activities)
                .ThenInclude(e => e.Pictograms)
                //And return it
                .FirstOrDefaultAsync();

            return user;
        }
        public async Task<int> DeleteSpecificWeek(GirafUser user, Week week)
        {
            user.WeekSchedule.Remove(week);
            return await Context.SaveChangesAsync();

        }
        public async Task<int> UpdateSpecificWeek(Week week)
        {
            Context.Weeks.Update(week);
            return await Context.SaveChangesAsync();
        }
        //This and AddPictogramsToWeekday should actually be seperated in the correct repositories but they were together in the same class when i moved them, and SetWeekFromDTO calls AddpictoramstoWeekday
        // I do not want to instantiate a repository in a repository, or rewrite them at the moment so here they are.
        /// <summary>
        /// From the given DTO, set the name, thumbnail and days of the given week object.
        /// </summary>
        /// <param name="weekDTO">The DTO from which values are read.</param>
        /// <param name="week">The week object to which values are written.</param>
        /// <returns>MissingProperties if thumbnail is missing.
        /// ResourceNotFound if any pictogram id is invalid.
        /// null otherwise.</returns>
        /// The 2 functions where static for somereason when they where located in sharedmethods.
        /// They should probably be changed to something simpler
        public async Task<ErrorResponse> SetWeekFromDTO(WeekBaseDTO weekDTO, WeekBase week)
        {
            var modelErrorCode = weekDTO.ValidateModel();
            if (modelErrorCode.HasValue)
            {
                return new ErrorResponse(modelErrorCode.Value, "Invalid model");
            }

            week.Name = weekDTO.Name;

            Pictogram thumbnail = Context.Pictograms
                .FirstOrDefault(p => p.Id == weekDTO.Thumbnail.Id);
            if (thumbnail == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "Missing thumbnail");

            week.Thumbnail = thumbnail;

            foreach (var day in weekDTO.Days)
            {
                var wkDay = new Weekday(day);
                if (!(await AddPictogramsToWeekday(wkDay, day)))
                {
                    return new ErrorResponse(ErrorCode.ResourceNotFound, "Missing pictogram");
                }

                week.UpdateDay(wkDay);
            }

            // All week days that were not specified in the new schedule, but existed before
            var toBeDeleted = week.Weekdays.Where(wd => !weekDTO.Days.Any(d => d.Day == wd.Day)).ToList();
            foreach (var deletedDay in toBeDeleted)
            {
                week.Weekdays.Remove(deletedDay);
            }

            return null;
        }

        /// <summary>
        /// Take pictograms and choices from DTO and add them to weekday object.
        /// </summary>
        /// <returns>True if all pictograms and choices were found and added, and false otherwise.</returns>
        /// <param name="to">Pictograms and choices will be added to this object.</param>
        /// <param name="from">Pictograms and choices will be read from this object.</param>
        public async Task<bool> AddPictogramsToWeekday(Weekday to, WeekdayDTO from)
        {
            if (from.Activities != null)
            {
                foreach (var activityDTO in from.Activities)
                {

                    List<Pictogram> pictograms = new List<Pictogram>();

                    foreach (var pictogram in activityDTO.Pictograms)
                    {
                        var picto = await Context.Pictograms
                            .Where(p => p.Id == pictogram.Id).FirstOrDefaultAsync();

                        if (picto != null)
                        {
                            pictograms.Add(picto);
                        }
                        else
                        {
                            return false;
                        }
                    }

                    Timer timer = null;
                    if (activityDTO.Timer != null)
                    {
                        timer = await Context.Timers.FirstOrDefaultAsync(t => t.Key == activityDTO.Timer.Key);
                    }

                    if (pictograms.Any())
                        to.Activities.Add(new Activity(to, pictograms, activityDTO.Order, activityDTO.State, timer, activityDTO.IsChoiceBoard, activityDTO.Title));
                }
            }
            return true;
        }
    }
}

