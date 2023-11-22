using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GirafRepositories.WeekPlanner
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