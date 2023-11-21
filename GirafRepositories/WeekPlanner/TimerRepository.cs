using GirafEntities.WeekPlanner;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using Microsoft.EntityFrameworkCore;
using Timer = GirafEntities.WeekPlanner.Timer;

namespace GirafRepositories.WeekPlanner
{
    public class TimerRepository : Repository<Timer>, ITimerRepository
    {
        public TimerRepository(GirafDbContext context) : base(context)
        {
        }

        public Task<Timer> getActivitysTimerkey(Activity activity)
        {
            return Context.Timers.FirstOrDefaultAsync(t => t.Key == activity.TimerKey);

        }
        public Task<Timer> getTimerWithKey(long? Key)
        {
            return Context.Timers.FirstOrDefaultAsync(t => t.Key == Key);
        }

    }
}