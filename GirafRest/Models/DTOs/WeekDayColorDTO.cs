using System;
using GirafRest.Models;

namespace GirafRest
{
    public class WeekDayColorDTO
    {
        public WeekDayColorDTO()
        {
        }

        /// <summary>
        /// Color as a Hex string
        /// </summary>
        public string HexColor { get; set; }

        /// <summary>
        /// Day for which the color belongs
        /// </summary>
        /// <value>The day.</value>
        public Days Day { get; set; }

    }
}
