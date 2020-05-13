using GirafRest.Models;

namespace GirafRest
{
    /// <summary>
    /// Color of a weekday DTO
    /// </summary>
    public class WeekDayColorDTO
    {
        /// <summary>
        /// Empty constructor used by JSON Generation
        /// </summary>
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
