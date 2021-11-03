using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using GirafRest.Interfaces;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafRest.Models.GirafUser>
    {
        bool ExistsUsername(string username);
        GirafUser GetUserByUsername(string username);
        IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users);
        GirafUser GetUserByID(string id);
        GirafUser GetUserWithId(string id);
        void Update(GirafUser user);
        Task<int> SaveChangesAsync();
        GirafUser CheckIfUserExists(string id);
        GirafUser CheckIfCallerOwnsResource(string id);
        GirafUser GetCitizensWithId(string id);
        GirafUser GetFirstCitizen(GuardianRelation citizen);
        GirafUser GetGuardianWithId(string id);
        GirafUser GetGuardianFromRelation(GuardianRelation guardian);
        GirafUser GetCitizenRelationship(string citizenId);
        GirafUser GetGuardianRelationship(string id);
        GirafUser GetUserSettingsByWeekDayColor(string id);
        bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user);
    }
    
}
