namespace GirafEntities.WeekPlanner.DTOs
{
    /// <summary>
    /// Data Transfer Object for <see cref="WeekTemplate"/>
    /// </summary>
    public class WeekTemplateDTO : WeekBaseDTO
    {
        /// <summary>
        /// JSON Generation requires empty constructor
        /// </summary>
        public WeekTemplateDTO() : base()
        {
        }

        /// <summary>
        /// Constructor for WeekTemplate DTO
        /// </summary>
        /// <param name="weekTemplate">WeekTemplate used as base for DTO</param>
        public WeekTemplateDTO(WeekTemplate weekTemplate) : base(weekTemplate)
        {
            this.DepartmentKey = weekTemplate.DepartmentKey;
            this.Id = weekTemplate.Id;
        }

        /// <summary>
        /// Key of the department the template belongs to
        /// </summary>
        public long DepartmentKey { get; internal set; }

        /// <summary>
        /// Id of the corresponding <see cref="WeekTemplate"/>
        /// </summary>
        public long Id { get; set; }
    }
}
