using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models;

namespace GirafRest
{
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

        /// A reference to the actual user.
        /// </summary>
        [ForeignKey("CitizenId")]
        public virtual GirafUser Citizen { get; set; }

        //A reference to the actual resource.
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
