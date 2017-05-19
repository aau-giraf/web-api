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
        public long ThumbnailKey { get; set; }
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
            initWeek();
        }

        /// <summary>
        /// A constructor for week setting only the thumbnail.
        /// </summary>
        public Week(Pictogram thumbnail)
        {
            this.Thumbnail = thumbnail;
            this.Weekdays = new Weekday[7] { new Weekday(), new Weekday(), new Weekday(), new Weekday(), new Weekday(), new Weekday(), new Weekday()};
        }
        /// <summary>
        /// Creates a new Week from the given WeekDTO.
        /// </summary>
        /// <param name="weekDTO">The data transfer object to create a new week from.</param>
        public Week(WeekDTO weekDTO)
        {
            initWeek();
            if(weekDTO.Days != null){
                foreach (var day in weekDTO.Days)
                {
                    UpdateDay(new Weekday(day));
                }
            }
            this.ThumbnailKey = weekDTO.Thumbnail.Id;
        }

        /// <summary>
        /// Overrides the data of this week with the data of the given DTO.
        /// </summary>
        /// <param name="weekDTO">New data.</param>
        public void Merge(WeekDTO weekDTO)
        {
            initWeek();
            if(weekDTO.Days != null){
                foreach (var day in weekDTO.Days)
                {
                    UpdateDay(new Weekday(day));
                }
            }
            if (weekDTO.Id != null)
                this.Id = (long)weekDTO.Id;
            else
                this.Id = 0;
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
        /// <summary>
        /// Initialises the week. Must be initialised like this, otherwise the Weekdays will not receive a key
        /// </summary>
        public void initWeek()
        {
            this.Weekdays = new Weekday[7] 
            { 
                new Weekday() { Day = Days.Monday }, 
                new Weekday() { Day = Days.Tuesday }, 
                new Weekday() { Day = Days.Wednesday }, 
                new Weekday() { Day = Days.Thursday }, 
                new Weekday() { Day = Days.Friday }, 
                new Weekday() { Day = Days.Saturday }, 
                new Weekday() { Day = Days.Sunday }};
        }
    }
}