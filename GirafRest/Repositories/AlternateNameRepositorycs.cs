using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    /// <inheritdoc cref="GirafRest.IRepositories.IAlternateNameRepository" />
    public class AlternateNameRepository : Repository<AlternateName>, IAlternateNameRepository
    {
        /// <summary>
        /// Domain specific repository implementation facade for the DBContext.
        /// </summary>
        /// <param name="context">The context to operate on</param>
        public AlternateNameRepository(GirafDbContext context)
            : base(context)
        { }

        /// <inheritdoc />
        public AlternateName GetForUser(string userId, long PictogramId)
        {
            // This is better than a predicate because the prictogramId is indexed.
            if (TryGet(out AlternateName entity, PictogramId) &&
                entity.Citizen.Id == userId) {
                return entity;
            }
            return default;
        }
    }
}
