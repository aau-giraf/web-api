using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class GirafUserRepository : Repository<GirafUser>, IGirafUserRepository
    {
        public GirafUserRepository(GirafDbContext context) : base(context)
        {
        }


        public bool ExistsUsername(string username)
            => Context.Users.Any(u => u.UserName == username);

        public GirafUser GetUserByUsername(string username)
            => Context.Users.FirstOrDefault(u => u.UserName == username);

        public List<GirafUser> GetListOfUsersByIdAndDep(GirafUser user, IQueryable<string> userIds)
        {
           return Context.Users.Where(u => userIds.Any(ui => ui == u.Id)
                                             && u.DepartmentKey == user.DepartmentKey).ToList();
        }
    }
}