using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class PictogramRelationRepository : Repository<PictogramRelation>, IPictogramRelationRepository
    {
        public PictogramRelationRepository(GirafDbContext context) : base(context)
        {

        }
    }
}