using System;
using System.Diagnostics.CodeAnalysis;

namespace GirafRest
{
    /// <summary>
    /// DTO for weekname, holding year, number and name
    /// </summary>
    public class WeekNameDTO : IComparable<WeekNameDTO>
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

        public int CompareTo( WeekNameDTO other)
        {
            if (this.WeekYear.CompareTo(other.WeekYear) != 0)
            {
                return -1*this.WeekYear.CompareTo(other.WeekYear);
            }else
            {
                return -1*this.WeekNumber.CompareTo(other.WeekNumber);
            }
               
           
        }
    }
}
