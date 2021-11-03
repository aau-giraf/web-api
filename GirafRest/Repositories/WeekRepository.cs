using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class WeekRepository : Repository<Week>, IWeekRepository
    {
        public WeekRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
