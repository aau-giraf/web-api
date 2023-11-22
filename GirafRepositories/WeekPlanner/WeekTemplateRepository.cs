using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class WeekTemplateRepository : Repository<WeekTemplate>, IWeekTemplateRepository
    {
        public WeekTemplateRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
