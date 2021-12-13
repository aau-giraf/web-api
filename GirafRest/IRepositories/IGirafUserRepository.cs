using GirafRest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using GirafRest.Models.DTOs;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafUser> { 
        /// <summary>
        /// Fetches the first or default (null) User by ID
        /// </summary>
        /// <param name="userID">The ID of the user to fetch</param>
        /// <returns>The User instance or default</returns>
        GirafUser GetByID(string userID);       
        Task<GirafUser> GetUserWithId(string id);
        Task<int> SaveChangesAsync();
        GirafUser CheckIfUserExists(string id);
        GirafUser GetCitizensWithId(string id);
        GirafUser GetFirstCitizen(GuardianRelation citizen);
        GirafUser GetGuardianWithId(string id);
        GirafUser GetGuardianFromRelation(GuardianRelation guardian);
        GirafUser GetCitizenRelationship(string citizenId);
        GirafUser GetUserSettingsByWeekDayColor(string id);
        public bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user);
        public Task<GirafUser> LoadUserWithResources(GirafUser usr);
        public Task<GirafUser> LoadUserWithDepartment(GirafUser usr);
        public Task<GirafUser> LoadUserWithWeekSchedules(string id);
        public Task<GirafUser> LoadBasicUserDataAsync(GirafUser usr);
        public GirafUser GetWithWeekSchedules(string id);
        bool ExistsUsername(string username);
        Task<GirafUser> GetUserByUsername(string username);
        IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users);
    }
    
}
