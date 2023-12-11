using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class WeekdayRepository : Repository<Weekday>, IWeekdayRepository
    {
        public WeekdayRepository(GirafDbContext context) : base(context)
        {

        }

        public async Task<int> DeleteSpecificWeekDay(Weekday oldDay)
        {
            Context.Weekdays.Update(oldDay);
            return await Context.SaveChangesAsync();
        }
        public async Task<int> UpdateSpecificWeekDay(Weekday oldDay)
        {
            Context.Weekdays.Update(oldDay);
            return await Context.SaveChangesAsync();
        }
    }
}
