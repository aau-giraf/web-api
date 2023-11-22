using GirafEntities.Responses;
using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using Microsoft.EntityFrameworkCore;
using Timer = GirafEntities.WeekPlanner.Timer;

namespace GirafRepositories.WeekPlanner
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
    }
}

