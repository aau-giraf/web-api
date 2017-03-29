using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models
{
    public class Week
    {
        public Weekday[] days { get; set; }
        public long Key { get; set; }

        public Week()
        {
            days = new Weekday[7]; // 7 days in this array
        }
    }
}