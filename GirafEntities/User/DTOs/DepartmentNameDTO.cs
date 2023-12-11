namespace GirafEntities.User.DTOs
{
    /// <summary>
    /// DTO for Department Name
    /// </summary>
    public class DepartmentNameDTO
    {
        /// <summary>
        /// The id of the department.
        /// </summary>
        public long ID { get; set; }
        /// <summary>
        /// The name of the department.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Constructor for ID and Name
        /// </summary>
        public DepartmentNameDTO(long id, string name)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// Empty constructor for JSON Generation
        /// </summary>
        public DepartmentNameDTO() { }
    }
}
