using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafRest.Models.GirafUser>
    {
        bool ExistsUsername(string username);
        GirafUser GetUserByUsername(string username);
        IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users);
        public GirafUser GetWithWeekSchedules(string id);
        GirafUser GetUserByID(string id);
    }

}
