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
        public long Key { get; set; }

        /// <summary>
        /// The key of the weekday to which the resource is attached.
        /// </summary>
        [Required]
        public long OtherKey { get; set; }
        /// <summary>
        /// A reference to the actual weekday.
        /// </summary>
        [ForeignKey("OtherKey")]
        public virtual Weekday Other { get; set; }

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
        /// Nullable key for TimerKey
        /// </summary>
        public long? TimerKey { get; set; }

        /// <summary>
        /// A reference to the actual timer.
        /// </summary>
        [ForeignKey("TimerKey")]
        public virtual Timer Timer { get; set; }

        /// <summary>
        /// State of the Activity.
        /// </summary>
        [Required]
        public ActivityState State { get; set; }

        /// <summary>
        /// Ordering
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a weekday and a resource.
        /// </summary>
        /// <param name="weekday">The involved weekday.</param>
        /// <param name="pictogram">The activity's pictogram.</param>
        /// <param name="order">The activity's order.</param>
        /// <param name="state">The activity's current state.</param>
        public Activity(Weekday weekday, Pictogram pictogram, int order, ActivityState state, Timer timer)
        {
            this.Other = weekday;
            this.PictogramKey = pictogram.Id;
            this.Pictogram = pictogram;
            this.Order = order;
            this.State = state;
            this.Timer = timer;
        }

        /// <summary>
        /// Newtonsoft (JSON Generation) needs empty constructor. Don't delete.
        /// </summary>
        public Activity() { }
    }
}