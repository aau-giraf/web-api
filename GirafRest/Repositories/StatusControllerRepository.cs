using GirafRest.Controllers;
using GirafRest.Data;
using GirafRest.Interfaces;
using GirafRest.IRepositories;
using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class StatusControllerRepository : Repository<GirafUser>, IStatusControllerRepository
    {
        
        public StatusControllerRepository(IGirafService giraf, GirafDbContext context) : base(context)
        {
        }
        //Checks connection to database through context
        public virtual async Task<Boolean> CheckDbConnectionAsync()
        {
            return await Context.Database.CanConnectAsync();
        }

    }
}
