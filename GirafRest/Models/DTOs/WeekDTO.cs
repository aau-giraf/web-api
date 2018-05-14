using GirafRest.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    public class WeekDTO : WeekBaseDTO
    {
        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; internal set; }
        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; internal set; }

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