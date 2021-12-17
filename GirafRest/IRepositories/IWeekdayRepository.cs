using GirafRest.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GirafRest.IRepositories
{
    public interface IWeekdayRepository : IRepository<Weekday>
    {
        public Task<int> DeleteSpecificWeekDay(Weekday oldDay);
        public void Update(Weekday weekday);
        public Task<int> UpdateSpecificWeekDay(Weekday oldDay);
    }
}
