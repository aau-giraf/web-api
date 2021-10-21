using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
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
        GirafUser GetSettingsWithId(string id);
        void Update(GirafUser user);
        Task<int> SaveChangesAsync();
        Task<byte[]> ReadRequestImage(Stream bodyStream);
        GirafUser CheckIfUserExists(string id);
        Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);
        Task<GirafUser> LoadBasicUserDataAsync();
        Task<bool> CheckPrivateOwnership(Pictogram resource, GirafUser curUsr);
        GirafUser CheckIfCallerOwnsResource(string id);
        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);
        Task<UserResource> FetchRelationshipFromDb(Pictogram resource, GirafUser user);
        void Remove(UserResource relationship);
        GirafUser GetCitizensWithId(string id);
        GirafUser GetFirstCitizen(GuardianRelation citizen);
        GirafUser GetGuardianWithId(string id);
        GirafUser GetGuardian(GuardianRelation guardian);
        GirafUser GetCitizenRelationship(string citizenId);
        GirafUser GetGuardianRelationship(string id);
        GirafUser GetUserSettingsByWeekDayColor(string id);
    }
    
}
