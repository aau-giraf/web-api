using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IAlternateNameRepository : IRepository<GirafRest.Models.AlternateName>
    {
        public Task<AlternateName> Get(GirafUser user, long pictogramID);
    }
}