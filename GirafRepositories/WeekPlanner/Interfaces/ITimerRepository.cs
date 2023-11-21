using GirafEntities.WeekPlanner;
using Timer = GirafEntities.WeekPlanner.Timer;

namespace GirafRepositories.Interfaces
{
    public interface ITimerRepository : IRepository<Timer>
    {
        public Task<Timer> getActivitysTimerkey(Activity activity);
        public Task<Timer> getTimerWithKey(long? Key);

    }
}