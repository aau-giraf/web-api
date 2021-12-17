using GirafRest.Data;
using GirafRest.IRepositories;
using GirafRest.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Repositories
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