using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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