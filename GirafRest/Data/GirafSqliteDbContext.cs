using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Data
{
    public class GirafSqliteDbContext : GirafDbContext
    {
        public GirafSqliteDbContext(DbContextOptions<GirafSqliteDbContext> options) : base (options)
        {

        }
    }
}
