using System.Collections.Generic;
using System.Linq;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;
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