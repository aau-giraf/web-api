using GirafRest.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IStatusControllerRepository : IRepository<StatusController>
    {
        public Task GetUserDbSetAsync();
    }
}
