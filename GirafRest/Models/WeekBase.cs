using GirafRest.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirafRest.Models
{
    public class WeekBase
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
        public WeekBase()
        {
            this.Weekdays = new List<Weekday>();
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public WeekBase(Pictogram thumbnail) : this()
        {
            this.Thumbnail = thumbnail;
        }

        /// <summary>
        /// Creates a new Week from the given WeekDTO.
        /// </summary>
        /// <param name="weekDTO">The data transfer object to create a new week from.</param>
        public WeekBase(WeekDTO weekDTO) : this()
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
            var wd = Weekdays.FirstOrDefault(d => d.Day == day.Day);
            if (wd == null)
                Weekdays.Add(day);
            else
                wd.Activities = day.Activities;
        }
    }
}
