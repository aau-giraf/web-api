using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class WeekBaseRepository : Repository<WeekBase>, IWeekBaseRepository
    {
        public WeekBaseRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
