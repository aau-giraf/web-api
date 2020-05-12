using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    /// <summary>
    /// Departments group users and thus have a list of users. They may own resources, that are available
    /// to all users in the department.
    /// </summary>
    public class Department {
        /// <summary>
        /// Primary key
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        /// <summary>
        /// Department name
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// All belonging members
        /// </summary>
        public virtual ICollection<GirafUser> Members { get; set; }

        /// <summary>
        /// All belongings resources
        /// </summary>
        public virtual ICollection<DepartmentResource> Resources { get; }

        /// <summary>
        /// All belonging week templates
        /// </summary>
        public virtual ICollection<WeekTemplate> WeekTemplates { get; }

        /// <summary>
        /// Empty contructor for JSON Generation
        /// </summary>
        public Department () {
            Members = new List<GirafUser>();
            Resources = new List<DepartmentResource>();
        }

        /// <summary>
        /// Creates a new department from the given department DTO.
        /// </summary>
        /// <param name="depDTO">The DTO containing all data on the new department.</param>
        public Department(DepartmentDTO depDTO) : this (){
            if (depDTO == null) {
                throw new System.ArgumentNullException(depDTO + " is null");
            }
            this.Name = depDTO.Name;
        }
    }
}