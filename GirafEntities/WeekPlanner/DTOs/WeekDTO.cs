namespace GirafEntities.WeekPlanner.DTOs
{
    /// <summary>
    /// DTO for <see cref="Week"/>
    /// </summary>
    public class WeekDTO : WeekBaseDTO
    {
        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; set; }

        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// Empty contstructor used by JSON Generation
        /// </summary>
        public WeekDTO()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="week">Week used as base</param>
        public WeekDTO(Week week) : base(week)
        {
            this.WeekYear = week.WeekYear;
            this.WeekNumber = week.WeekNumber;
        }
    }
}