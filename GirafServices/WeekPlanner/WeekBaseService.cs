using GirafEntities.WeekPlanner;
using Microsoft.SqlServer.Management.Smo.Agent;

namespace GirafServices.WeekPlanner
{
    public class WeekBaseService
    {
        /// <summary>
        /// Updates the given weekday of the Week with the new information found in 'day'.
        /// </summary>
        /// <param name="day">A day instance to update the week with - the old one is completely overridden.</param>
        public void UpdateDay(Weekday day)
        {
            var wd = WeekDays.FirstOrDefault(d => d.Day == day.Day);
            if (wd == null)
                Weekdays.Add(day);
            else
                wd.Activities = day.Activities;
        }
    }
}
