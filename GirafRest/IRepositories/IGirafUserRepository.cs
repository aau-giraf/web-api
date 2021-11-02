using GirafRest.Models;

namespace GirafRest.IRepositories
{
    /// <summary>
    /// Domain specific repository for fetching users and user view models
    /// </summary>
    public interface IGirafUserRepository : IRepository<GirafRest.Models.GirafUser>
    { 
        /// <summary>
        /// Fetches the first or default (null) User by ID
        /// </summary>
        /// <param name="userID">The ID of the user to fetch</param>
        /// <returns>The User instance or default</returns>
        GirafUser GetByID(string userID);
    }
}
