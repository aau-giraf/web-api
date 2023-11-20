using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Repositories
{
    public class PictogramRelationRepository : Repository<PictogramRelation>, IPictogramRelationRepository
    {
        public PictogramRelationRepository(GirafDbContext context) : base(context)
        {

        }

        public ICollection<PictogramRelation> GetWithPictogram(long activityID)
            => Context.PictogramRelations
                .Include(pictogram => pictogram.Pictogram)
                .Where(pr => pr.ActivityId == activityID)
                .ToList();
    }
}