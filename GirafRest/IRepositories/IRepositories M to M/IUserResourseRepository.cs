using GirafRest.Models;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IUserResourseRepository : IRepository<UserResource>
    {
        Task<int> AddAsync(UserResource userResource);
        Task<UserResource> FetchRelationshipFromDb(Pictogram resource, GirafUser user);
        public Task<bool> CheckPrivateOwnership(Pictogram pictogram, GirafUser user);
        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user);
    }
}