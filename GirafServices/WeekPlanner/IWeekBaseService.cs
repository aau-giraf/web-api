using GirafEntities.WeekPlanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirafServices.WeekPlanner
{
    public interface IWeekBaseService
    {
        void UpdateDay(Weekday day, WeekBase wb);
    }
}
