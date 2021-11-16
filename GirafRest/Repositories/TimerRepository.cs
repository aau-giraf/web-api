using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GirafRest.Repositories
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

    }
}