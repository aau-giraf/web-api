using System;
namespace GirafRest
{
    public class WeekNameDTO
    {
        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; set; }

        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; set; }

        public WeekNameDTO()
        {

        }

        public WeekNameDTO(int weekYear, int weekNumber, string name)
        {
            this.Name = name;
            this.WeekYear = weekNumber;
            this.WeekNumber = weekNumber;
        }
    }
}
