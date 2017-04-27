using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Defines a many-to-many relationship between department and resource.
    /// </summary>
    public class DepartmentResource : IManyToMany<long, Department>
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        /// <summary>
        /// The key of the involved department.
        /// </summary>
        [Required]
        public long OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual department.
        /// </summary>
        [ForeignKey("OtherKey")]
        public virtual Department Other {get; set;}

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        [Required]
        public long ResourceKey { get; set; }
        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("ResourceKey")]
        public virtual Frame Resource { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a department and a resource.
        /// </summary>
        /// <param name="dep">The involved department.</param>
        /// <param name="resource">The involved resource.</param>
        public DepartmentResource(Department dep, Frame resource)
        {
            this.OtherKey = dep.Key;
            this.Other = dep;
            this.ResourceKey = resource.Id;
            this.Resource = resource;

            Other.Resources.Add(this);
            resource.Departments.Add(this);
        }

        /// <summary>
        /// DO NOT DELETE THIS!
        /// </summary>
        public DepartmentResource(){}
    }
}