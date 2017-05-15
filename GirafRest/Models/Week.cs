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
        public long ThumbnailKey { get; set; }
        [ForeignKey("ThumbnailKey")]
        public Pictogram Thumbnail { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Week()
        {
            this.Weekdays = new List<Weekday>();
        }
        public Week(Pictogram thumbnail)
        {
            this.Thumbnail = thumbnail;
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
            if (weekDTO.Id != null)
                this.Id = (long)weekDTO.Id;
            else
                this.Id = -1;
        }

        /// <summary>
        /// Initializes the weekdays of the week.
        /// </summary>
        public void InitWeek() 
        {
            if(!Weekdays.Any())
            {
                for(int i = 0; i < 7; i++){
                    this.Weekdays.Add(
                        new Weekday()
                        {
                            Day = (Days) i
                        });
                }
            }
        }
    }
}