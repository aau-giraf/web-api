using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.Many_to_Many_Relationships
{
    /// <summary>
    /// Defines the many-to-many relationship between Choice and Resource
    /// </summary>
    public class ChoiceResource : IManyToMany<long, Choice>
    {
        /// <summary>
        /// The key of the relationship
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        /// <summary>
        /// The key of the involved resource.
        /// </summary>
        [Required]
        public long ResourceKey { get; set; }
        /// <summary>
        /// A reference to the actual resource.
        /// </summary>
        [ForeignKey("ResourceKey")]
        public Resource Resource { get; set; }

        /// <summary>
        /// The key of the involved choice.
        /// </summary>
        [Required]
        public long OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual choice.
        /// </summary>
        [ForeignKey("ResourceKey")]
        public Choice Other { get; set; }

        /// <summary>
        /// Creates a new many-to-many relation between the given choice and the given resource.
        /// </summary>
        /// <param name="choice">The involved choice.</param>
        /// <param name="resource">The involved resource.</param>
        public ChoiceResource(Choice choice, Resource resource)
        {
            this.OtherKey = choice.Id;
            this.Other = choice;
            this.ResourceKey = resource.Id;
            this.Resource = resource;
        }
        
        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public ChoiceResource() { }
    }
}
