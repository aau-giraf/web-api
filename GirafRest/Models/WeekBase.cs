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
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }

        public IList<Weekday> Weekdays { get; set; }

        public long ThumbnailKey { get; set; }
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

        public WeekBase(Pictogram thumbnail) : this()
        {
            this.Thumbnail = thumbnail;
        }
    }
}
