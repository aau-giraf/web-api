using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GirafEntities.User;
using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;
using GirafRest.Models;
using GirafRest.Data;
using Microsoft.EntityFrameworkCore;

namespace GirafRepositories.User
{
    public class DepartmentResourseRepository : Repository<DepartmentResource>, IDepartmentResourseRepository
    {
        public DepartmentResourseRepository(GirafDbContext context) : base(context)
        {
        }
        public async Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user)
        {
            return await Context.DepartmentResources
                .Where(dr => dr.PictogramKey == resource.Id && dr.OtherKey == user.Department.Key)
                .AnyAsync();
        }

        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user)
        {
            return Context.DepartmentResources
                .Any(dr => dr.PictogramKey == pictogram.Id
                           && dr.OtherKey == user.DepartmentKey);
        }
    }
}