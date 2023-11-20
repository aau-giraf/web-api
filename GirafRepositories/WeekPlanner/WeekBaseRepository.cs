using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;

namespace GirafRepositories.WeekPlanner
{
    public class WeekBaseRepository : Repository<WeekBase>, IWeekBaseRepository
    {
        public WeekBaseRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
