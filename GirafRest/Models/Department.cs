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
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<GirafUser> Members { get; set; }

        public virtual ICollection<DepartmentResource> Resources { get; set; }

        public virtual ICollection<WeekTemplate> WeekTemplates { get; set; }

        // DO NOT DELETE
        public Department () {
            Members = new List<GirafUser>();
            Resources = new List<DepartmentResource>();
        }

        /// <summary>
        /// Creates a new department from the given department DTO.
        /// </summary>
        /// <param name="depDTO">The DTO containing all data on the new department.</param>
        public Department(DepartmentDTO depDTO) : this (){
            this.Name = depDTO.Name;
        }
    }
}