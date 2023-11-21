using GirafRest.Models;

namespace GirafRepositories.Interfaces
{
    public interface IWeekdayRepository : IRepository<GirafRest.Models.Weekday>
    {
        public Task<int> DeleteSpecificWeekDay(Weekday oldDay);
        public Task<int> UpdateSpecificWeekDay(Weekday oldDay);
    }
}