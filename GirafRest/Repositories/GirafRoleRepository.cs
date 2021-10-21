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
        {
        }

        public string GetRoleGuardianId()
        {
           return Context.Roles.Where(r => r.Name == GirafRole.Guardian)
                                .Select(c => c.Id).FirstOrDefault();
           
        }
    }
}