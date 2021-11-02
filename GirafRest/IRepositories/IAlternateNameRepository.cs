using GirafRest.Models;

namespace GirafRest.IRepositories
{
    /// <summary>
    /// Domain specific repository for querying Alternate names for pictograms
    /// </summary>
    public interface IAlternateNameRepository : IRepository<AlternateName>
    {
        /// <summary>
        /// Fetches the instance or default (null) of a <see cref="AlternateName"/>
        ///   if no name was found for the <see cref="GirafUser"/> and <see cref="Pictogram"/>
        /// </summary>
        /// <param name="userId">The ID of the <see cref="GirafUser"/> which has the <see cref="AlternateName"/> for the <see cref="Pictogram"/></param>
        /// <param name="PictogramId">The ID of the <see cref="Pictogram"/></param>
        /// <returns><see cref="AlternateName"/> or default, if not found</returns>
        AlternateName GetForUser(string userId, long PictogramId);
    }
}