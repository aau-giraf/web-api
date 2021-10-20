using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafRest.Models.GirafUser> {
        public Task<GirafUser> LoadUserWithWeekSchedules(string id);
    }
}
