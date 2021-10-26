using System.Collections.Generic;
using System.Linq;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;

namespace GirafRest.Repositories
{
    public class WeekdayRepository : Repository<Weekday>, IWeekdayRepository
    {
        public WeekdayRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
