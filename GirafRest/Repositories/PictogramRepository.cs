using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    /// <inheritdoc cref="GirafRest.IRepositories.IPictogramRepository"/>
    public class PictogramRepository : Repository<Pictogram>, IPictogramRepository
    {
        /// <summary>
        /// Domain specific repository implementation facade for the DBContext.
        /// </summary>
        /// <param name="context">The context to operate on</param>
        public PictogramRepository(GirafDbContext context)
            : base(context)
        { }

        /// <inheritdoc />
        public Pictogram GetByID(long pictogramID)
            => Get(pictogramID);
    }
}