using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class WeekDayColorRepository : Repository<WeekDayColor>, IWeekDayColorRepository
    {
        public WeekDayColorRepository(GirafDbContext context) : base(context)
        {

        }
    }
}
