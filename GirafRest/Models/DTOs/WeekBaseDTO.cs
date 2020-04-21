using GirafRest.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="WeekBase"/>
    /// </summary>
    public class WeekBaseDTO
    {
        /// <summary>
        /// The weeks thumbnail.
        /// </summary>
        public WeekPictogramDTO Thumbnail { get; set; }

        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of the days in the week schedule.
        /// </summary>
        public ICollection<WeekdayDTO> Days { get; set; }

        /// <summary>
        /// Creates a new data transfer object for a given week.
        /// </summary>
        /// <param name="week">The week to create a DTO for.</param>
        public WeekBaseDTO(WeekBase week)
        {
            this.Name = week.Name;
            try
            {
                this.Thumbnail = new WeekPictogramDTO(week.Thumbnail);
            }
            catch (NullReferenceException)
            {
                this.Thumbnail = new WeekPictogramDTO();
            }
            Days = new List<WeekdayDTO>();
            foreach (var day in week.Weekdays.OrderBy(wd => wd.Day))
            {
                Days.Add(new WeekdayDTO(day));
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR.
        /// </summary>
        public WeekBaseDTO() { }

        /// <summary>
        ///  Validates the WeekDTO for e.g. amount of days
        /// </summary>
        /// <returns>InvalidAmountOfWeekdays if amount of days is not in the range 1 to 7.
        /// TwoDaysCannotHaveSameDayProperty if we e.g. have two Thursdays in a single week.</returns>
        public ErrorCode? ValidateModel()
        {
            if (this.Days == null || (this.Days.Count < 1 || this.Days.Count > 7))
                return ErrorCode.InvalidAmountOfWeekdays;
            if (this.Days.Any(d => !Enum.IsDefined(typeof(Days), d.Day)))
                return ErrorCode.InvalidDay;
            //If two days have the same day index
            if (this.Days.GroupBy(d => d.Day).Any(g => g.Count() != 1))
                return ErrorCode.TwoDaysCannotHaveSameDayProperty;
            return null;
        }
    }
}
