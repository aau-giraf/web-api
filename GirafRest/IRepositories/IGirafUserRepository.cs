using GirafRest.Models;
using System;
using System.Collections.Generic;
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
    public interface IGirafUserRepository : IRepository<GirafUser> {

        
        Task<GirafUser> GetUserWithId(string id);
        void Update(GirafUser user);
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
