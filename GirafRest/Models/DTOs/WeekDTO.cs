using System;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    public class WeekDTO
    {
        public long Id { get; set; }
        public ICollection<WeekdayDTO> Days { get; set; }
        public WeekDTO(Week week)
        {
            this.Id = week.Id;
            Days = new List<WeekdayDTO>();
            foreach (var day in week.Weekdays)
            {
                Days.Add(new WeekdayDTO(day));
            }
        }

        public WeekDTO() {}
    }
}