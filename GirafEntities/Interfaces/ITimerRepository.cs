using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface ITimerRepository : IRepository<GirafRest.Models.Timer>
    {
        public Task<Timer> getActivitysTimerkey(Activity activity);
        public Task<Timer> getTimerWithKey(long? Key);

    }
}