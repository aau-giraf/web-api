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
        IEnumerable<string> GetUsersWithRole(string role);
        IEnumerable<string> GetAllGuardians();
    }
}