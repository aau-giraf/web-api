using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafRest.IRepositories;
using GirafRest.Data; 

namespace GirafRest.Repositories
{
    public class TrusteeRelationRepository : Repository<TrusteeRelation>, ITrusteeRelationRepository
    {
        public TrusteeRelationRepository(GirafDbContext context) : base(context)
        {

        }

    }
}
