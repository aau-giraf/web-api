using GirafRest.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Week when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class WeekDTO : WeekBaseDTO
    {
        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; set; }
        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; set; }

        public WeekDTO()
        {

        }

        public WeekDTO(Week week) : base(week)
        {
            this.WeekYear = week.WeekYear;
            this.WeekNumber = week.WeekNumber;
        }
    }
}