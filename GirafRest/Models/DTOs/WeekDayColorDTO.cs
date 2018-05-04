using System;
using GirafRest.Models;

namespace GirafRest
{
    public class WeekDayColorDTO
    {
        public WeekDayColorDTO()
        {
        }

        public string HexColor { get; set; }

        public Days Day { get; set; }

    }
}
