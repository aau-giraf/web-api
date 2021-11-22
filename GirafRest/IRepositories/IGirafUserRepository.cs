using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IGirafUserRepository : IRepository<GirafUser> {
        /// <summary>
        /// Loads the user with week schedules.
        /// </summary>
        /// <returns>The user with week schedules.</returns>
        /// <param name="id">Identifier of <see cref="GirafUser"/></param>
        public GirafUser GetWithWeekSchedules(string id);
        bool ExistsUsername(string username);
        GirafUser GetUserByUsername(string username);
        IEnumerable<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users);
        GirafUser GetUserWithId(string id);
    }
}
