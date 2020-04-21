using System;
namespace GirafRest
{
    /// <summary>
    /// DTO for weekname, holding year, number and name
    /// </summary>
    public class WeekNameDTO
    {
        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; internal set; }

        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; internal set; }

        /// <summary>
        /// Empty constructor for JSON Generation
        /// </summary>
        public WeekNameDTO()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WeekNameDTO(int weekYear, int weekNumber, string name)
        {
            this.Name = name;
            this.WeekYear = weekYear;
            this.WeekNumber = weekNumber;
        }
    }
}
