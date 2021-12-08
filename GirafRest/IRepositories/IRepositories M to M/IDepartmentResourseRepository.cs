using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GirafRest.Models;

namespace GirafRest.IRepositories
{
    public interface IDepartmentResourseRepository : IRepository<DepartmentResource>
    {
        public Task<bool> CheckProtectedOwnership(Pictogram resource, GirafUser user);
        public bool CheckIfUserOwnsResource(Pictogram pictogram, GirafUser user);
    }
}