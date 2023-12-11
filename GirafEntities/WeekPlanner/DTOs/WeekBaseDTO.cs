using GirafEntities.Responses;

namespace GirafEntities.WeekPlanner.DTOs
{
    /// <summary>
    /// DTO for <see cref="WeekBase"/>
    /// </summary>
    public class WeekBaseDTO
    {
        /// <summary>
        /// The weeks thumbnail.
        /// </summary>
        public WeekPictogramDTO Thumbnail { get; set; }

        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of the days in the week schedule.
        /// </summary>
        public ICollection<WeekdayDTO> Days { get; set; }

        /// <summary>
        /// Creates a new data transfer object for a given week.
        /// </summary>
        /// <param name="week">The week to create a DTO for.</param>
        public WeekBaseDTO(WeekBase week)
        {
            this.Name = week.Name;
            try
            {
                this.Thumbnail = new WeekPictogramDTO(week.Thumbnail);
            }
            catch (NullReferenceException)
            {
                this.Thumbnail = new WeekPictogramDTO();
            }
            Days = new List<WeekdayDTO>();
            foreach (var day in week.Weekdays.OrderBy(wd => wd.Day))
            {
                Days.Add(new WeekdayDTO(day));
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR.
        /// </summary>
        public WeekBaseDTO() { }
        
    }
}
