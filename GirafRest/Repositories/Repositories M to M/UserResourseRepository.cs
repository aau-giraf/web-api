using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class UserResourseRepository : Repository<UserResource>, IUserResourseRepository
    {
        public UserResourseRepository(GirafDbContext context) : base(context)
        {

        }
    }
}