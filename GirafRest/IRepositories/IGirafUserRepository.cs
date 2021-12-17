using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafUser>
    {

        /// <summary>
        /// Fetches the first or default (null) User by ID
        /// </summary>
        /// <param name="userID">The ID of the user to fetch</param>
        /// <returns>The User instance or default</returns>
        GirafUser GetByID(string userID);

        GirafUser GetUserWithId(string id);

        void Update(GirafUser user);

        Task<int> SaveChangesAsync();

        GirafUser CheckIfUserExists(string id);

        GirafUser GetCitizensWithId(string id);

        GirafUser GetFirstCitizen(GuardianRelation citizen);

        GirafUser GetGuardianWithId(string id);

        GirafUser GetGuardianFromRelation(GuardianRelation guardian);

        GirafUser GetCitizenRelationship(string citizenId);

        GirafUser GetUserSettingsByWeekDayColor(string id);

        bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user);

        Task<GirafUser> LoadUserWithResources(GirafUser usr);

        Task<GirafUser> LoadUserWithDepartment(GirafUser usr);

        Task<GirafUser> LoadUserWithWeekSchedules(string id);

        Task<GirafUser> LoadBasicUserDataAsync(GirafUser usr);

        GirafUser GetWithWeekSchedules(string id);

        bool ExistsUsername(string username);

        GirafUser GetUserByUsername(string username);

        IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users);

        //GirafUser GetUserWithId(string id);

        Task<GirafUser> GetUserWithIdOrUsername(DisplayNameDTO member);
    }
}
