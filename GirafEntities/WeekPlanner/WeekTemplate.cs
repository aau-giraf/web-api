using GirafEntities.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafEntities.WeekPlanner
{
    /// <summary>
    /// Week template for a week in a given department
    /// </summary>
    public class WeekTemplate : WeekBase
    {
        /// <summary>
        /// Reference to the owning department
        /// </summary>
        [ForeignKey("Department")]
        public long DepartmentKey { get; set; }

        /// <summary>
        /// A reference to the department using this template.
        /// </summary>
        public virtual Department Department { get; set; }

        /// <summary>
        /// Empty contructor is used for NewtonSoft JSON generation.
        /// </summary>
        public WeekTemplate()
        {
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public WeekTemplate(Department department)
        {
            DepartmentKey = department?.Key ?? -1;
        }
    }
}
