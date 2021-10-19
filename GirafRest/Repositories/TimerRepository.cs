using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class TimerRepository : Repository<Timer>, ITimerRepository
    {
        public TimerRepository(GirafDbContext context) : base(context)
        {
        }

        public int CHANGENAME()
        _context.Timers.FirstOrDefault(t => t.Key == activity.TimerKey);
    }
}