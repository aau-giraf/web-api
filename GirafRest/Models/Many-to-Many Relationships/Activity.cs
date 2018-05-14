using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GirafRest.Models
{
    /// <summary>
    /// Defines a many-to-many relationship between <see cref="Weekday"/> and <see cref="Pictogram"/> (called resource).
    /// </summary>
    public class Activity
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
        [ForeignKey("PictogramKey")]
        public virtual Pictogram Pictogram { get; set; }

        [Required]
        public ActivityState State { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a weekday and a resource.
        /// </summary>
        /// <param name="weekday">The involved weekday.</param>
        /// <param name="resource">The involved resource.</param>
        public Activity(Weekday weekday, Pictogram pictogram, int order, ActivityState state)
        {
            this.Other = weekday;
            this.PictogramKey = pictogram.Id;
            this.Pictogram = pictogram;
            this.Order = order;
            this.State = state;
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Activity(){}
    }
}