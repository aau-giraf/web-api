using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Data
{
    public class GirafMySqlDbContext : GirafDbContext
    {
        public GirafMySqlDbContext(DbContextOptions<GirafMySqlDbContext> options) : base(options)
        {

        }
    }
}
