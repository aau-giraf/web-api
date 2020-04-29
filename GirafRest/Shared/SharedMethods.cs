using System.Linq;
using System.Threading.Tasks;
using GirafRest.Models;
using GirafRest.Models.DTOs;
using GirafRest.Models.Responses;
using GirafRest.Services;
using Microsoft.EntityFrameworkCore;

namespace GirafRest.Shared
{
    /// <summary>
    /// Shared static class for helper methods
    /// </summary>
    public static class SharedMethods
    {
        /// <summary>
        /// From the given DTO, set the name, thumbnail and days of the given week object.
        /// </summary>
        /// <param name="weekDTO">The DTO from which values are read.</param>
        /// <param name="week">The week object to which values are written.</param>
        /// <param name="_giraf">An instance of the GirafService from which the database will be accessed when reading the DTO.</param>
        /// <returns>MissingProperties if thumbnail is missing.
        /// ResourceNotFound if any pictogram id is invalid.
        /// null otherwise.</returns>
        public static async Task<ErrorResponse> SetWeekFromDTO(WeekBaseDTO weekDTO, WeekBase week, IGirafService _giraf)
        {
            var modelErrorCode = weekDTO.ValidateModel();
            if (modelErrorCode.HasValue)
            {
                return new ErrorResponse(modelErrorCode.Value, "Invalid model");
            }
            
            week.Name = weekDTO.Name;
            
            Pictogram thumbnail = _giraf._context.Pictograms
                .FirstOrDefault(p => p.Id == weekDTO.Thumbnail.Id);
            if(thumbnail == null)
                return new ErrorResponse(ErrorCode.MissingProperties, "Missing thumbnail");

            week.Thumbnail = thumbnail;

            foreach (var day in weekDTO.Days)
            {
                var wkDay = new Weekday(day);
                if (!(await AddPictogramsToWeekday(wkDay, day, _giraf)))
                {
                    return new ErrorResponse(ErrorCode.ResourceNotFound, "Missing pictogram");
                }

                week.UpdateDay(wkDay);
            }

            // All week days that were not specified in the new schedule, but existed before
            var toBeDeleted = week.Weekdays.Where(wd => !weekDTO.Days.Any(d => d.Day == wd.Day)).ToList();
            foreach (var deletedDay in toBeDeleted)
            {
                week.Weekdays.Remove(deletedDay);
            }

            return null;
        }

        /// <summary>
        /// Take pictograms and choices from DTO and add them to weekday object.
        /// </summary>
        /// <returns>True if all pictograms and choices were found and added, and false otherwise.</returns>
        /// <param name="to">Pictograms and choices will be added to this object.</param>
        /// <param name="from">Pictograms and choices will be read from this object.</param>
        /// <param name="_giraf">IGirafService for injection.</param>
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