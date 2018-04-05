using System.Collections.Generic;
using System.Linq;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of a Department when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class DepartmentDTO
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
        /// A list of the UserNames of all members of the department.
        /// </summary>
        public ICollection<string> Members { get; set; }
        /// <summary>
        /// A list of ids of all resources owned by the department.
        /// </summary>
        public ICollection<long> Resources { get; set; }

        /// <summary>
        /// Creates a new department data transfer object from a given department.
        /// </summary>
        /// <param name="department">The department to transfer.</param>
        public DepartmentDTO(Department department)
        {
            this.ID = department.Key;
            this.Name = department.Name;
            this.Members = new List<string> (department.Members.Select(m => m.UserName));
            this.Resources = new List<long> (department.Resources.Select(dr => dr.ResourceKey));
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public DepartmentDTO ()
        {
            Members = new List<string>();
            Resources = new List<long>();
        }
    }
}