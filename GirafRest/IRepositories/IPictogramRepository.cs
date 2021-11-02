namespace GirafRest.IRepositories
{
    /// <summary>
    /// Domain specific repository for pictograms
    /// </summary>
    public interface IPictogramRepository : IRepository<Models.Pictogram>
    {
        /// <summary>
        /// Fetches the first or default (null) Pictogram by ID
        /// </summary>
        /// <param name="pictogramID">The ID of the pictogram to fetch</param>
        /// <returns>The Pictogram instance or default</returns>
        Models.Pictogram GetByID(long pictogramID);
    }
}