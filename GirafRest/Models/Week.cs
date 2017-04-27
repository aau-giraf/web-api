using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    /// <summary>
    /// A week defines the schedule of some citizen in the course of the week. A week schedule may be used
    /// across several actual weeks.
    /// </summary>
    public class Week
    {
        /// <summary>
        /// The id of the week.
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; private set; }

        /// <summary>
        /// A collection of weekdays for each day of the week.
        /// </summary>
        public ICollection<Weekday> Weekdays { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Week()
        {
            this.Weekdays = new List<Weekday>();
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
            this.Weekdays = new List<Weekday>();
            foreach (var day in weekDTO.Days)
            {
                this.Weekdays.Add(new Weekday(day));
            }
            this.Id = weekDTO.Id;
        }

        /// <summary>
        /// Initializes the weekdays of the week.
        /// </summary>
        public void InitWeek() 
        {
            if(!Weekdays.Any())
            {
                this.Weekdays.Add(new Weekday(Days.Monday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Tuesday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Wednesday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Thursday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Friday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Saturday, null, new List<Frame>()));
                this.Weekdays.Add(new Weekday(Days.Sunday, null, new List<Frame>()));
            }
        }
    }
}