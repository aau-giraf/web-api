using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafRest.Models;

namespace GirafRepositories.Interfaces
{
    public interface IDepartmentResourseRepository : IRepository<DepartmentResource>
    {
        public Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user);
        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user);
    }
}