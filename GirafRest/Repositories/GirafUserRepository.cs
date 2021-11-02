using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    /// <inheritdoc cref="GirafRest.IRepositories.IGirafUserRepository" />
    public class GirafUserRepository : Repository<GirafUser>, IGirafUserRepository
    {
        /// <summary>
        /// Domain specific repository implementation facade for the DBContext.    
        /// </summary>
        /// <param name="context">The context to operate on</param>
        public GirafUserRepository(GirafDbContext context)
            : base(context)
        { }

        /// <inheritdoc />
        public GirafUser GetByID(string userID) 
            => Get(userID);
    }
}