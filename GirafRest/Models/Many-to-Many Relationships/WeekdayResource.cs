using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    public class WeekdayResource : IManyToMany<long, Weekday>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        [Required]
        public long OtherKey { get; set; }
        [ForeignKey("OtherKey")]
        public virtual Weekday Other {get; set;}

        [Required]
        public long ResourceKey { get; set; }
        [ForeignKey("ResourceKey")]
        public virtual Frame Resource { get; set; }

        public WeekdayResource(Weekday weekday, Frame resource)
        {
            this.OtherKey = weekday.Key;
            this.Other = weekday;
            this.ResourceKey = resource.Key;
            this.Resource = resource;

            //weekday.Elements.Add(this);
        }

        public WeekdayResource(){}
    }
}