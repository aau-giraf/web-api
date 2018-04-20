using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Defines a many-to-many relationship between weekday and resource.
    /// </summary>
    public class WeekdayResource
    {
        /// <summary>
        /// The key of the relationship entity.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Key {get; set;}

        /// <summary>
        /// The key of the weekday to which the resource is attached.
        /// </summary>
        [Required]
        public long OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual weekday.
        /// </summary>
        [ForeignKey("OtherKey")]
        public virtual Weekday Other {get; set;}

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

        public int Order { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a weekday and a resource.
        /// </summary>
        /// <param name="weekday">The involved weekday.</param>
        /// <param name="resource">The involved resource.</param>
        public WeekdayResource(Weekday weekday, Pictogram pictogram, int order)
        {
            this.Other = weekday;
            this.PictogramKey = pictogram.Id;
            this.Pictogram = pictogram;
            this.Order = order;
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public WeekdayResource(){}
    }
}