using GirafRest.Controllers;
using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IStatusControllerRepository : IRepository<GirafUser>
    {
        public Task<Boolean> CheckDbConnectionAsync();
    }
}
