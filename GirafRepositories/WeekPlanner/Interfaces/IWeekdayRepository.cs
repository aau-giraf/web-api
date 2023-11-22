using GirafEntities.WeekPlanner;

namespace GirafRepositories.Interfaces
{
    public interface IWeekdayRepository : IRepository<Weekday>
    {
        public Task<int> DeleteSpecificWeekDay(Weekday oldDay);
        public Task<int> UpdateSpecificWeekDay(Weekday oldDay);
    }
}