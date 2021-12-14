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
        /// <param name="pictogramId">The ID of the <see cref="Pictogram"/></param>
        /// <returns><see cref="AlternateName"/> or default, if not found</returns>
        AlternateName GetForUser(string userId, long pictogramId);

        /// <summary>
        ///  If the <see cref="AlternateName"/> has already been created, it will return true.
        /// </summary>
        /// <param name="userId">The ID of the <see cref="GirafUser"/> which has the <see cref="AlternateName"/> for the <see cref="Pictogram"/></param>
        /// <param name="pictogramId">The ID of the <see cref="Pictogram"/></param>
        /// <returns>boolean</returns>
        bool UserAlreadyHas(string userId, long pictogramId)
            => GetForUser(userId, pictogramId) != null;
    }
}