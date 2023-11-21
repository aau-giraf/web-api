using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;

namespace GirafRepositories.WeekPlanner
{
    public class WeekDayColorRepository : Repository<WeekDayColor>, IWeekDayColorRepository
    {
        public WeekDayColorRepository(GirafDbContext context) : base(context)
        {

        }
    }
}
