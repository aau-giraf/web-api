using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;

namespace GirafRepositories.WeekPlanner
{
    public class WeekTemplateRepository : Repository<WeekTemplate>, IWeekTemplateRepository
    {
        public WeekTemplateRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
