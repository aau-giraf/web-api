using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace GirafRest.Models
{
    /// <summary>
    /// Base model for defining a Week
    /// </summary>
    public class WeekBase
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Name of the week
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of weekdays
        /// </summary>
        public IList<Weekday> Weekdays { get; set; }

        /// <summary>
        /// Thumbnail key for week
        /// </summary>
        public long ThumbnailKey { get; set; }

        /// <summary>
        /// Thumbnail
        /// </summary>
        [ForeignKey("ThumbnailKey")]
        public virtual Pictogram Thumbnail { get; set; }

        /// <summary>
        /// Updates the given weekday of the Week with the new information found in 'day'.
        /// </summary>
        /// <param name="day">A day instance to update the week with - the old one is completely overridden.</param>
        public void UpdateDay(Weekday day)
        {
            var wd = Weekdays.FirstOrDefault(d => d.Day == day.Day);
            if (wd == null)
                Weekdays.Add(day);
            else
                wd.Activities = day.Activities;
        }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public WeekBase()
        {
            this.Weekdays = new List<Weekday>();
        }

        /// <summary>
        /// Constructor for WeekBase
        /// </summary>
        public WeekBase(Pictogram thumbnail) : this()
        {
            this.Thumbnail = thumbnail;
        }
    }
}
