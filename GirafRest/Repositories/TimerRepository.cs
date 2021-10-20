using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Repositories
{
    public class TimerRepository : Repository<Timer>, ITimerRepository
    {
        public TimerRepository(GirafDbContext context) : base(context)
        {
        }

        public Task getActivityTimerkey(Activity activity)
        {
            Context.Timers.FirstOrDefaultAsync(t => t.Key == activity.TimerKey);

        }

    }
}