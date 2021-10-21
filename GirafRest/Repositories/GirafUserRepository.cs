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

        GirafUser GetUserByID(long userID) 
            => return Context.Users.FirstOrDefaultAsync(us => us.Id == userId);
    }
}