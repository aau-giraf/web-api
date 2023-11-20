using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IWeekdayRepository : IRepository<GirafRest.Models.Weekday>
    {
        public Task<int> DeleteSpecificWeekDay(Weekday oldDay);
        public Task<int> UpdateSpecificWeekDay(Weekday oldDay);
    }
}