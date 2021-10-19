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

        public GirafUser GetUserByUserID(string userId)
            => Context.Users.FirstOrDefault(u => u.Id == userId);

        public void RemoveUser(GirafUser user)
            => Context.Users.Remove(user);
    }
}