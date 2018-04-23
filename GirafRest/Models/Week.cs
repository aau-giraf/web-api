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
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A collection of weekdays for each day of the week.
        /// </summary>
        public IList<Weekday> Weekdays { get; set; }

        /// <summary>
        /// The year of the week.
        /// </summary>
        public int WeekYear { get; set; }
        /// <summary>
        /// The number of the week, 0 - 52 (53).
        /// </summary>
        public int WeekNumber { get; set; }

        /// <summary>
        /// The key of the weeks Thumbnail.
        /// </summary>
        public long ThumbnailKey { get; set; }
        [ForeignKey("ThumbnailKey")]

        /// <summary>
        /// The thumbnail for the week.
        /// </summary>
        public virtual Pictogram Thumbnail { get; set; }

        /// <summary>
        /// Empty constructor required by Newtonsoft testing framework.
        /// </summary>
        public Week()
        {
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public Week(Pictogram thumbnail)
        {
            this.Thumbnail = thumbnail;
        }

        /// <summary>
        /// Creates a new Week from the given WeekDTO.
        /// </summary>
        /// <param name="weekDTO">The data transfer object to create a new week from.</param>
        public Week(WeekDTO weekDTO)
        {
            this.Name = weekDTO.Name;
            if (weekDTO.Days != null)
            {
                foreach (var day in weekDTO.Days)
                {
                    UpdateDay(new Weekday(day));
                }
            }
            this.ThumbnailKey = weekDTO.Thumbnail.Id;
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