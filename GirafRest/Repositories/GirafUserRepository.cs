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

        public List<GirafUser> GetUsersInDepartment(long departmentKey, IEnumerable<string> users)
        {
           return Context.Users
            .Where(user => 
                // Checks if the user is a guardian
                users.Any(currUser => currUser == user.Id) &&
                user.DepartmentKey != null && user.DepartmentKey == departmentKey)
            .ToList();
        }

        public GirafUser GetUserByID(string id)
            => Context.Users.FirstOrDefault(u => u.Id == id);
    }
}