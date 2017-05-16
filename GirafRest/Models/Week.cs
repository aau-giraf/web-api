using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    /// <summary>
    /// A week defines the schedule of some citizen in the course of the week.
    /// </summary>
    public class Week
    {
        /// <summary>
        /// The id of the week.
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// A collection of weekdays for each day of the week.
        /// </summary>
        public IList<Weekday> Weekdays { get; set; }
        
        /// <summary>
        /// The key of the weeks Thumbnail.
        /// </summary>
        public long? ThumbnailKey { get; set; }
        [ForeignKey("ThumbnailKey")]

        /// <summary>
        /// The thumbnail for the week.
        /// </summary>
        public virtual Pictogram Thumbnail { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Week()
        {
            this.Weekdays = new List<Weekday>();
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public Week(Pictogram thumbnail)
        {
            this.Thumbnail = thumbnail;
            this.Weekdays = new Weekday[7];
        }
        /// <summary>
        /// Creates a new Week from the given WeekDTO.
        /// </summary>
        /// <param name="weekDTO">The data transfer object to create a new week from.</param>
        public Week(WeekDTO weekDTO)
        {
            Merge(weekDTO);
        }

        /// <summary>
        /// Overrides the data of this week with the data of the given DTO.
        /// </summary>
        /// <param name="weekDTO">New data.</param>
        public void Merge(WeekDTO weekDTO)
        {
            this.Weekdays = new Weekday[7];
            foreach (var day in weekDTO.Days)
            {
                UpdateDay(new Weekday(day));
            }
            if (weekDTO.Id != null)
                this.Id = (long)weekDTO.Id;
            else
                this.Id = 0;
            this.ThumbnailKey = weekDTO.ThumbnailID;
        }

        /// <summary>
        /// Updates the given weekday of the Week with the new information found in 'day'.
        /// </summary>
        /// <param name="day">A day instance to update the week with - the old one is completely overridden.</param>
        public void UpdateDay(Weekday day)
        {
            Weekdays[(int)day.Day] = day;
        }
    }
}