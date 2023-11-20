using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRepositories.User
{
    public class GuardianRelationRepository : Repository<GuardianRelation>, IGuardianRelationRepository
    {
        public GuardianRelationRepository(GirafDbContext context) : base(context)
        {

        }
    }
}