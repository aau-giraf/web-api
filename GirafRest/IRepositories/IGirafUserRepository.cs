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

        GirafUser GetUserByUserID(string userId);

        List<GirafUser> GetListOfUsersByIdAndDep(GirafUser user, IQueryable<string> userIds);
        void RemoveUser(GirafUser user);
    }
}
