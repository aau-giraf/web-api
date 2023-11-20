using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class GirafRoleRepository : Repository<GirafRole>, IGirafRoleRepository
    {
        public GirafRoleRepository(GirafDbContext context) : base(context)
        { }

        public IEnumerable<string> GetUsersWithRole(string roleId)
            => Context.UserRoles
                .Where(identityRolePair => identityRolePair.RoleId == roleId)
                .Select(identityRolePair => identityRolePair.UserId).Distinct();

        public string GetGuardianRoleId()
            => Context.Roles
                .Where(r => r.Name == GirafRole.Guardian)
                .Select(c => c.Id).FirstOrDefault();
        
        public IEnumerable<string> GetAllGuardians()
            => GetUsersWithRole(GetGuardianRoleId());

        public string GetCitizenRoleId()
            => Context.Roles
                .Where(r => r.Name == GirafRole.Citizen)
                .Select(c => c.Id).FirstOrDefault();

        public IEnumerable<string> GetAllCitizens()
            => GetUsersWithRole(GetCitizenRoleId());
    }
}