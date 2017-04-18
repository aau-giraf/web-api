using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models.Many_to_Many_Relationships
{
    public class ChoiceResource : IManyToMany<long, Choice>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key { get; set; }

        [Required]
        public long ResourceKey { get; set; }
        [ForeignKey("ResourceKey")]
        public Frame Resource { get; set; }

        [Required]
        public long OtherKey { get; set; }
        [ForeignKey("ResourceKey")]
        public Choice Other { get; set; }

        public ChoiceResource(Choice choice, Frame resource)
        {
            this.OtherKey = choice.Id;
            this.Other = choice;
            this.ResourceKey = resource.Id;
            this.Resource = resource;
        }

        public ChoiceResource() { }
    }
}
