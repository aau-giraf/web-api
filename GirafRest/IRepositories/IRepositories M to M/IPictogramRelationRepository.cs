using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using GirafRest.Models; 

namespace GirafRest.IRepositories
{
    public interface IPictogramRelationRepository : IRepository<PictogramRelation>
    {
        public ICollection<PictogramRelation> GetWithPictogram(long activityID);
    }
}