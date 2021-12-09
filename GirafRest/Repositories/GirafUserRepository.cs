using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;


namespace GirafRest.Repositories
{
    /// <summary>
    /// A repository for giraf user
    /// </summary>
    public class GirafUserRepository : Repository<GirafUser>, IGirafUserRepository
    {

        public GirafUserRepository(GirafDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Fetches the first or default (null) User by ID
        /// </summary>
        /// <param name="userID">The ID of the user to fetch</param>
        /// <returns>The User instance or default</returns>
        public GirafUser GetByID(string userID) 
            => Get(userID);

        /// <summary>
        /// Gets first user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GirafUser> GetUserWithId(string id)
        {

            return await Context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Updates user.
        /// </summary>
        /// <param name="user"></param>
        public void Update(GirafUser user)
        {
            Context.Users.Update(user);
        }

        /// <summary>
        /// Saves changes.
        /// </summary>
        /// <returns></returns>
        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
        /// <summary>
        /// Attempt to find the target user and check that he exists.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser CheckIfUserExists(string id)
        {
            return Context.Users.Include(u => u.Resources).ThenInclude(dr => dr.Pictogram)
                .FirstOrDefault(u => u.Id == id);
        }
        
        /// <summary>
        /// Gets citizen users with matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Guardian User</returns>
        public GirafUser GetCitizensWithId(string id)
        {
            return Context.Users.Include(u => u.Citizens).FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Gets the first citizen user
        /// </summary>
        /// <param name="citizen"></param>
        /// <returns></returns>
        public GirafUser GetFirstCitizen(GuardianRelation citizen)
        {
            return Context.Users.FirstOrDefault(u => u.Id == citizen.CitizenId);
        }

        /// <summary>
        /// Finds guardians for the given giraf user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser GetGuardianWithId(string id)
        {
            return Context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Finds guardian in a given guardian relation.
        /// </summary>
        /// <param name="guardian"></param>
        /// <returns></returns>
        public GirafUser GetGuardianFromRelation(GuardianRelation guardian)
        {
            return Context.Users.FirstOrDefault(u => u.Id == guardian.GuardianId);
        }

        /// <summary>
        /// Find the guardian with given citizen-id.
        /// </summary>
        /// <param name="citizenId"></param>
        /// <returns>Citizen User</returns>
        public GirafUser GetCitizenRelationship(string citizenId)
        {
            return Context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == citizenId);
        }
        
        /// <summary>
        /// Gets user settings and weekday color on the user with the given id. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser GetUserSettingsByWeekDayColor(string id)
        {
            return Context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors)
                .FirstOrDefault(u => u.Id == id);
        }

        //check whether user with that username already exist that does not have the same id
        public bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user)
        {
            return Context.Users.Any(u => u.UserName == newUser.Username && u.Id != user.Id);
        }

        public async Task<GirafUser> GetUserByUsername(string username)
        {
            return await Context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<GirafUser> LoadUserWithResources(GirafUser usr)
        {
            return await Context.Users
                //Get user by ID from database
                .Where(u => u.Id == usr.Id)
                //Then load his pictograms - both the relationship and the actual pictogram
                .Include(u => u.Resources)
                .ThenInclude(ur => ur.Pictogram)
                //Then load his department and their pictograms
                .Include(u => u.Department)
                .ThenInclude(d => d.Resources)
                .ThenInclude(dr => dr.Pictogram)
                //And return it
                .FirstOrDefaultAsync(); 
        }

        public async Task<GirafUser> LoadUserWithDepartment(GirafUser usr)
        {
            return await Context.Users
                .Where(u => u.Id == usr.Id)
                .Include(u => u.Department)
                .FirstOrDefaultAsync();
        }

        public async Task<GirafUser> LoadUserWithWeekSchedules(string id)
        {
            return await Context.Users
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
        }
        
        /// Method for loading user from context and eager loading fields requied to read their <b>week schedules</b>
        /// <summary>
        /// <param name="id">id of user to load.</param>
        /// <returns>A <see cref="GirafUser"/> with <b>all</b> related data.</returns>
        /// </summary>
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

        public async Task<GirafUser> LoadBasicUserDataAsync(GirafUser usr)
        {
            return await Context.Users.Where(u => u.Id == usr.Id).FirstOrDefaultAsync();
        }
        
        public bool ExistsUsername(string username)
            => Context.Users.Any(u => u.UserName == username);

        public IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users)
        {
           return Context.Users
            .Where(user => 
                // Checks if the user is a guardian
                users.Any(currUser => currUser == user.Id) &&
                user.DepartmentKey != null && user.DepartmentKey == departmentKey)
            .ToList();
        }
    }
}