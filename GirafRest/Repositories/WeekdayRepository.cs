using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using GirafRest.Models;
using GirafRest.IRepositories;
using GirafRest.Data;
using System.Threading.Tasks;

namespace GirafRest.Repositories
{
    public class WeekdayRepository : Repository<Weekday>, IWeekdayRepository
    {
        public WeekdayRepository(GirafDbContext context) : base(context)
        {
        }
    }
}
