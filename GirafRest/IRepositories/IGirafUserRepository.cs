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
        public bool CheckIfUsernameHasSameId(GirafUserDTO newUser, GirafUser user);
    }
    
}
