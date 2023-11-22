using GirafEntities.WeekPlanner;

namespace GirafServices.WeekPlanner
{
    public class WeekBaseService : IWeekBaseService
    {
        /// <summary>
        /// Updates the given weekday of the Week with the new information found in 'day'.
        /// </summary>
        /// <param name="day">A day instance to update the week with - the old one is completely overridden.</param>
        public void UpdateDay(Weekday day, WeekBase wb)
        {
            var wd = wb.Weekdays.FirstOrDefault(d => d.Day == day.Day);
            if (wd == null)
                wb.Weekdays.Add(day);
            else
                wd.Activities = day.Activities;
        }
    }
}
