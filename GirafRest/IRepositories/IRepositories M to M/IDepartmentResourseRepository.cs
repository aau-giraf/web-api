using GirafRest.Models;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IDepartmentResourseRepository : IRepository<DepartmentResource>
    {
        public Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user);
        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user);
    }
}