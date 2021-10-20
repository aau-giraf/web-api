using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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

    }
}
