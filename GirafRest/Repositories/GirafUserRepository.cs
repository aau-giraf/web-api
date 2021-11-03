using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
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

        public GirafUser GetUserByID(string id)
            => Context.Users.FirstOrDefault(u => u.Id == id);
    }
        /// <summary>
        /// Gets first user.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser GetUserWithId(string id)
        {

            return Context.Users.FirstOrDefault(u => u.Id == id);
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
        /// Check if the caller owns the resource.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser CheckIfCallerOwnsResource(string id)
        {
            return Context.Users.Include(r => r.Resources).ThenInclude(dr => dr.Pictogram)
                .FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Gets citizen users with matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <returns></returns>
        public GirafUser GetCitizenRelationship(string citizenId)
        {
            return Context.Users.Include(u => u.Guardians).FirstOrDefault(u => u.Id == citizenId);
        }

        /// <summary>
        /// Gets first guardian with given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser GetGuardianRelationship(string id)
        {
            return Context.Users.FirstOrDefault(u => u.Id == id);
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

        public bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user)
        {
            return Context.Users.Any(u => u.UserName == newUser.Username && u.Id != user.Id);
        }
    
















}
}