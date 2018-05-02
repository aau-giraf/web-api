using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Controllers
{
    public static class SharedMethods
    {
        
        public static async Task<ErrorCode> UpdateWeekActivities(WeekBaseDTO newWeek, WeekBase week, IGirafService _giraf)
        {
            var modelErrorCode = newWeek.ValidateModel();
            if (modelErrorCode.HasValue)
            {
                return modelErrorCode.Value;
            }

            var orderedDays = week.Weekdays.OrderBy(w => w.Day).ToArray();
            foreach (var day in newWeek.Days)
            {
                var wkDay = new Weekday(day);
                if (!(await AddPictogramsToWeekday(wkDay, day, _giraf)))
                {
                    return ErrorCode.ResourceNotFound;
                }

                week.UpdateDay(wkDay);
            }

            //All week days that were not specified in the new schedule, but existed before
            var toBeDeleted = week.Weekdays.Where(wd => newWeek.Days.All(d => d.Day != wd.Day)).ToList();
            foreach (var deletedDay in toBeDeleted)
            {
                week.Weekdays.Remove(deletedDay);
            }

            return ErrorCode.NoError;
        }
        
        /// <summary>
        /// Take pictograms and choices from DTO and add them to weekday object.
        /// </summary>
        /// <returns>True if all pictograms and choices were found and added, and false otherwise.</returns>
        /// <param name="to">Pictograms and choices will be added to this object.</param>
        /// <param name="from">Pictograms and choices will be read from this object.</param>
        private static async Task<bool> AddPictogramsToWeekday(Weekday to, WeekdayDTO from, IGirafService _giraf){
            if(from.Activities != null) 
            {
                foreach (var activityDTO in from.Activities)
                {
                    var picto = await _giraf._context.Pictograms
                        .Where(p => p.Id == activityDTO.Pictogram.Id).FirstOrDefaultAsync();
                    
                    if (picto != null)
                        to.Activities.Add(new Activity(to, picto, activityDTO.Order, activityDTO.State));
                }
            }
            return true;
        }
    }
}