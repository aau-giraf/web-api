using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// DTO for <see cref="Weekday"/>
    /// </summary>
    public class WeekdayDTO
    {
        /// <summary>
        /// An enum defining which day of the week this Weekday represents.
        /// </summary>
        public Day Day { get; set; }
        
        /// <summary>
        /// A list of all the activities of the week.
        /// </summary>
        public ICollection<ActivityDTO> Activities { get; set; }
        
        /// <summary>
        /// Creates a new data transfer object for the given week.
        /// </summary>
        /// <param name="weekday">The weekday to create a DTO for.</param>
        public WeekdayDTO(Weekday weekday) {
            if (weekday == null) {
                throw new System.ArgumentNullException(weekday + " is null");
            }
            this.Day = weekday.Day;
            Activities = new List<ActivityDTO>();
            
            if(weekday.Activities != null){
                foreach (var elem in weekday.Activities.OrderBy(a => a.Order))
                {
                    if(elem.Pictogram != null) Activities.Add(new ActivityDTO(elem));
                }
            }
        }

        /// <summary>
        /// Empty constructor required for test framework.
        /// </summary>
        public WeekdayDTO() {}
    }
}