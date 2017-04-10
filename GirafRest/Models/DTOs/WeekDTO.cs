using System;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class WeekDTO
    {
        public ICollection<WeekdayDTO> Days { get; set; }
        public WeekDTO(Week week)
        {
            Days = new List<WeekdayDTO>();
            foreach (var day in week.Days)
            {
                Days.Add(new WeekdayDTO(day));
            }
        }

        public WeekDTO() {}
    }
}