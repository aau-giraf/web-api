using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    /// <summary>
    /// Domain specific repository for pictograms
    /// </summary>
    public interface IPictogramRepository : IRepository<Pictogram>
    {
        public Task<Pictogram> getPictogramMatchingRelation(PictogramRelation pictogramRelation);

        public Task<Pictogram> GetPictogramWithName(string name);

        /// <summary>
        /// Add empty pictogram with specified name and access level.
        /// saves to database.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="access"></param>
        public Task<int> AddPictogramWith_NO_ImageHash(string name, AccessLevel access);

        public Pictogram GetByID(long pictogramID);

        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);

        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);

        public Task<Pictogram> GetPictogramWithID(long Id);

        public IQueryable<Pictogram> fetchPictogramsFromDepartmentStartsWithQuery(string query, GirafUser user);

        public IQueryable<Pictogram> fetchPictogramsFromDepartmentsContainsQuery(string query, GirafUser user);

        public IQueryable<Pictogram> fetchPictogramsUserNotPartOfDepartmentStartsWithQuery(string query, GirafUser user);

        public IQueryable<Pictogram> fetchPictogramsUserNotPartOfDepartmentContainsQuery(string query, GirafUser user);

        public IQueryable<Pictogram> fetchPictogramsNoUserLoggedInStartsWithQuery(string query);

        public IQueryable<Pictogram> fetchPictogramsNoUserLoggedInContainsQuery(string query);
    }
}
