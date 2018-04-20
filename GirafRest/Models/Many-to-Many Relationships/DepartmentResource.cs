using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Defines a many-to-many relationship between department and resource.
    /// </summary>
    public class DepartmentResource
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
        public long PictogramKey { get; set; }
        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("ResourceKey")]
        public virtual Pictogram Pictogram { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a department and a resource.
        /// </summary>
        /// <param name="dep">The involved department.</param>
        /// <param name="resource">The involved resource.</param>
        public DepartmentResource(Department dep, Pictogram pictogram)
        {
            this.OtherKey = dep.Key;
            this.Other = dep;
            this.PictogramKey = pictogram.Id;
            this.Pictogram = pictogram;

            pictogram.LastEdit = DateTime.Now;
            Other.Resources.Add(this);
            pictogram.Departments.Add(this);
        }

        /// <summary>
        /// DO NOT DELETE THIS!
        /// </summary>
        public DepartmentResource(){}
    }
}