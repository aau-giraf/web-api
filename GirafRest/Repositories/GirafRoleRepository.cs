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
    }
}