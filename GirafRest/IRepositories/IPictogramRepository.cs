using GirafRest.Models;
using GirafRest.Models.DTOs;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    /// <summary>
    /// Domain specific repository for pictograms
    /// </summary>
    public interface IPictogramRepository : IRepository<Models.Pictogram>
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
        Task<Pictogram> FetchResourceWithId(ResourceIdDTO resourceIdDTO);
        public Task<Pictogram> FindResource(ResourceIdDTO resourceIdDTO);
        public Task<Pictogram> GetPictogramWithID(long Id);
        /// <summary>
        /// Fetches the first or default (null) Pictogram by ID
        /// </summary>
        /// <param name="pictogramID">The ID of the pictogram to fetch</param>
        /// <returns>The Pictogram instance or default</returns>
        Models.Pictogram GetByID(long pictogramID);
    }
}