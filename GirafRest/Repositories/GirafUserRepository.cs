using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class GirafUserRepository : Repository<GirafUser>, IGirafUserRepository
    {
        public GirafUserRepository(GirafDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Method for loading user from context and eager loading fields requied to read their <b>week schedules</b>
        /// </summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        public GirafUser GetWithWeekSchedules(string id)
            => Context.Users
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
                .FirstOrDefault();


        public bool ExistsUsername(string username)
            => Context.Users.Any(u => u.UserName == username);
        

        public GirafUser GetUserByUsername(string username)
            => Context.Users.FirstOrDefault(u => u.UserName == username);

        public IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users)
        {
           return Context.Users
            .Where(user => 
                // Checks if the user is a guardian
                users.Any(currUser => currUser == user.Id) &&
                user.DepartmentKey != null && user.DepartmentKey == departmentKey)
            .ToList();
        }

        public GirafUser GetUserWithId(string id)
            => Context.Users.FirstOrDefault(u => u.Id == id);
    }
}