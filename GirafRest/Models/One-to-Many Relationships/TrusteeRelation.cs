using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    /// <summary>
    /// THe Pivot table for one-to-many realtion between <see cref="Trustee"/> and <see cref="Citizen"/>
    /// </summary>
    public class TrusteeRelation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set;}
        
        /// <summary>
        /// The key for the relationship entity
        /// </summary>
        [Required]
        public string CitizenId { get; set; }

        /// <summary>
        /// THe key to the involved ressource
        /// </summary>
        [Required]
        public string TrusteeId { get; set; }

        /// <summary>
        /// A actual referance to the actual user
        /// </summary>
        [ForeignKey("CitizenId")]
        public virtual GirafUser Citizen { get; set; }


        /// <summary>
        /// A reference to the involved ressource 
        /// </summary>
        [ForeignKey("TrusteeId")]
        public virtual GirafUser Trustee { get; set; }


        /// <summary>
        /// Construtor
        /// </summary>
        /// <param name="trustee"></param>
        /// <param name="citizen"></param>
        public TrusteeRelation(GirafUser trustee, GirafUser citizen)
        {
            this.Citizen = citizen;
            this.Trustee = trustee; 
        }


        /// <summary>
        /// Emty constructor for Json Generation
        /// </summary>
        public TrusteeRelation()
        {
        }

    }
}
