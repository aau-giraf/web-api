using GirafRest.Models;

namespace GirafRepositories.Interfaces
{
    public interface IPictogramRelationRepository : IRepository<PictogramRelation>
    {
        public ICollection<PictogramRelation> GetWithPictogram(long activityID);
    }
}