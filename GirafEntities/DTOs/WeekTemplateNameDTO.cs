namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for <see cref="WeekTemplate"/>
    /// </summary>
    public class WeekTemplateNameDTO
    {
        /// <summary>Men 
        /// Name of the <see cref="WeekTemplate"/>
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Id of the <see cref="WeekTemplate"/>
        /// </summary>
        public long TemplateId { get; set; }

        /// <summary>
        /// Empty constructor used for JSON Generation
        /// </summary>
        public WeekTemplateNameDTO()
        {
        }

        /// <summary>
        /// Initialize a new DTO
        /// </summary>
        public WeekTemplateNameDTO(string name, long templateID)
        {
            this.Name = name;
            this.TemplateId = templateID;
        }
    }
}
