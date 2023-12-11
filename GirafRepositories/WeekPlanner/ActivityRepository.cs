using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class ActivityRepository : Repository<Activity>, IActivityRepository
    {
        public ActivityRepository(GirafDbContext context) : base(context)
        {

        }
    }
}