using System;
namespace GirafRest
{
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

        public DepartmentNameDTO(long id, string name)
        {
            this.ID = id;
            this.Name = name;
        }

    }
}
