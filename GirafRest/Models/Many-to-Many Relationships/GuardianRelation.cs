using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models;

namespace GirafRest
{
    /// <summary>
    /// Pivot table for many-many between <see cref="Guardian"/> and <see cref="Citizen"/>/>
    /// </summary>
    public class GuardianRelation
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        /// <summary>
        /// The key of the weekday to which the resource is attached.
        /// </summary>
        [Required]
        public string CitizenId { get; set; }

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        [Required]
        public string GuardianId { get; set; }

        /// <summary>
        /// A reference to the actual user.
        /// </summary>
        [ForeignKey("CitizenId")]
        public virtual GirafUser Citizen { get; set; }

        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("GuardianId")]
        public virtual GirafUser Guardian { get; set; }

        public GuardianRelation(GirafUser guardian, GirafUser citizen)
        {
            this.Citizen = citizen;
            this.Guardian = guardian;
        }

        public GuardianRelation()
        {

        }
    }
}
