namespace GirafRest
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
            this.ID = id;
            this.Name = name;
        }
        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;
            if (obj.GetType() != typeof(DepartmentNameDTO))
                return false;

            DepartmentNameDTO dto = (DepartmentNameDTO)obj;
            return this.ID == dto.ID && this.Name == dto.Name;  
        }


        /// <summary>
        /// Empty constructor for JSON Generation
        /// </summary>
        public DepartmentNameDTO() { }
    }
}
