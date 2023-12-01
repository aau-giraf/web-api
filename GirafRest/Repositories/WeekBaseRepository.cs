using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class WeekBaseRepository : Repository<WeekBase>, IWeekBaseRepository
    {
        public WeekBaseRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
