using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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

        public virtual ICollection<PictogramRelation> Pictograms { get; set; }
        
        public long? TimerKey { get; set; }

        /// <summary>
        /// A reference to the actual timer.
        /// </summary
        [ForeignKey("TimerKey")]
        public virtual Timer Timer { get; set; }

        [Required]
        public ActivityState State { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// Creates a new many-to-many relationship between a weekday and a resource.
        /// </summary>
        /// <param name="weekday">The involved weekday.</param>
        /// <param name="resource">The involved resource.</param>
        public Activity(Weekday weekday, List<Pictogram> pictograms, int order, ActivityState state)
        {
            this.Other = weekday;
            this.Order = order;
            this.State = state;
            this.Pictograms = new List<PictogramRelation>();
            AddPictograms(pictograms);
        }
        
        public Activity(Weekday weekday, int order, ActivityState state)
        {
            this.Other = weekday;
            this.Order = order;
            this.State = state;
            this.Pictograms = new List<PictogramRelation>();
        }

        

        public void AddPictogram(Pictogram pictogram) {
            this.Pictograms.Add(new PictogramRelation(this, pictogram));
        }

        public void AddPictograms(List<Pictogram> pictograms) {
            foreach (var pictogram in pictograms) 
            {
                AddPictogram(pictogram);
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Activity(){}
    }
}