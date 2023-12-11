using GirafEntities.WeekPlanner;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafEntities.User
{
    /// <summary>
    /// Defines a many-to-many relationship between <see cref="Department"/> and <see cref="Pictogram"/> (ressource)
    /// </summary>
    public class DepartmentResource
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        /// <summary>
        /// The key of the involved department.
        /// </summary>
        [Required]
        public long OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual department.
        /// </summary>
        [ForeignKey("OtherKey")]
        public virtual Department Other { get; set; }

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        [Required]
        public long PictogramKey { get; set; }
        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("PictogramKey")]
        public virtual Pictogram Pictogram { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a department and a resource.
        /// </summary>
        /// <param name="dep">The involved department.</param>
        /// <param name="pictogram">The involved pictogram.</param>
        public DepartmentResource(Department dep, Pictogram pictogram)
        {
            OtherKey = dep.Key;
            Other = dep;
            PictogramKey = pictogram.Id;
            Pictogram = pictogram;

            pictogram.LastEdit = DateTime.Now;
            Other.Resources.Add(this);
            pictogram.Departments.Add(this);
        }

        /// <summary>
        /// DO NOT DELETE THIS!
        /// </summary>
        public DepartmentResource() { }
    }
}