using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IWeekRepository : IRepository<GirafRest.Models.Week>
    {
        public Task<GirafUser> getAllWeeksOfUser(string userId);

        Task<GirafUser> LoadUserWithWeekSchedules(string userId);
        public Task<int> DeleteSpecificWeek(GirafUser user, Week week)
    }
}