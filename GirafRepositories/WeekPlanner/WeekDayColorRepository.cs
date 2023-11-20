using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class WeekDayColorRepository : Repository<WeekDayColor>, IWeekDayColorRepository
    {
        public WeekDayColorRepository(GirafDbContext context) : base(context)
        {

        }
    }
}
