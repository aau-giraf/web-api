using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;


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
            return Context.Users.Include(u => u.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
        }
        
        /// <summary>
        /// Check if the caller owns the resource.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GirafUser CheckIfCallerOwnsResource(string id)
        {
            return Context.Users.Include(r => r.Resources).ThenInclude(dr => dr.Pictogram).FirstOrDefault(u => u.Id == id);
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
            return Context.Users.Include(u => u.Settings).ThenInclude(w => w.WeekDayColors).FirstOrDefault(u => u.Id == id);
        }

       














    }
}