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
        /// The id of the department entity.
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        /// <summary>
        /// The name of the department.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// A collection of members associated with the department.
        /// </summary>
        public virtual ICollection<GirafUser> Members { get; set; }
        /// <summary>
        /// A collection of all resources owned by the department.
        /// </summary>
        public virtual ICollection<DepartmentResource> Resources { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Department () {
            Members = new List<GirafUser>();
            Resources = new List<DepartmentResource>();
        }

        /// <summary>
        /// Creates a new department from the given department DTO.
        /// </summary>
        /// <param name="depDto">The DTO containing all data on the new department.</param>
        public Department(DepartmentDTO depDto) : this (){
            this.Name = depDto.Name;
        }
    }
}