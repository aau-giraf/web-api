using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using GirafRest.Models;

namespace GirafRest.IRepositories
{
    public interface IUserResourseRepository : IRepository<UserResource>
    {
        Task<int> AddAsync(UserResource userResource);
        Task<UserResource> FetchRelationshipFromDb(Pictogram resource, GirafUser user);
        void Remove(UserResource relationship);
    }
}