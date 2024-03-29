using GirafEntities.User;
using GirafEntities.WeekPlanner;
using GirafEntities.WeekPlanner.DTOs;

namespace GirafRepositories.Interfaces
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

        public Task<Pictogram> GetPictogramsById(long pictogramID);

        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);

        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);

        public Task<Pictogram> GetPictogramWithID(long Id);

        public Task RemoveRelations(Pictogram pict);

        public Task CreatePictorgram(Pictogram pict);

        public Task RemovePictogram(Pictogram pict);
        public Task UpdatePictogram(Pictogram pict);

        public Task SaveState();

        public IEnumerable<Pictogram> fetchPictogramsFromDepartmentStartsWithQuery(string query, GirafUser user);

        public IEnumerable<Pictogram> fetchPictogramsFromDepartmentsContainsQuery(string query, GirafUser user);

        public IEnumerable<Pictogram> fetchPictogramsUserNotPartOfDepartmentStartsWithQuery(string query, GirafUser user);

        public IEnumerable<Pictogram> fetchPictogramsUserNotPartOfDepartmentContainsQuery(string query, GirafUser user);

        public IEnumerable<Pictogram> fetchPictogramsNoUserLoggedInStartsWithQuery(string query);

        public IEnumerable<Pictogram> fetchPictogramsNoUserLoggedInContainsQuery(string query);
    }
}