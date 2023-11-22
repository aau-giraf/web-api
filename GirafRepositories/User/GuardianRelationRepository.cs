using System.Collections.Generic;
using System.Linq;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Data;
using GirafEntities.User;

namespace GirafRepositories.User
{
    public class GuardianRelationRepository : Repository<GuardianRelation>, IGuardianRelationRepository
    {
        public GuardianRelationRepository(GirafDbContext context) : base(context)
        {

        }
    }
}