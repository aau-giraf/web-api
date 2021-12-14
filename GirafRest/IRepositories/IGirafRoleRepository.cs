using GirafRest.Models;
using GirafRest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace GirafRest.IRepositories
{
    public interface IGirafRoleRepository : IRepository<GirafRest.Models.GirafRole>
    {
        string GetGuardianRoleId();
        string GetCitizenRoleId();
        string GetTrusteeRoleId();
        IEnumerable<string> GetUsersWithRole(string role);
        IEnumerable<string> GetAllGuardians();
        IEnumerable<string> GetAllCitizens();
        IEnumerable<string> GetAllTrustees();
    }
}