using GirafRest.Models;
using System.Collections.Generic;

namespace GirafRest.IRepositories
{
    public interface IPictogramRelationRepository : IRepository<PictogramRelation>
    {
        public ICollection<PictogramRelation> GetWithPictogram(long activityID);
    }
}