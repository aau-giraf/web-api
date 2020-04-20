using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    /// <summary>
    /// Possible days; 1 for Monday
    /// </summary>
    public enum Days {
        /// <summary>
        /// Monday as 1
        /// </summary>
        Monday = 1,
        /// <summary>
        /// Tuesday as 2
        /// </summary>
        Tuesday = 2,
        /// <summary>
        /// Wednesday as 3
        /// </summary>
        Wednesday = 3,
        /// <summary>
        /// Thursday as 4
        /// </summary>
        Thursday = 4,
        /// <summary>
        /// Friday as 5
        /// </summary>
        Friday = 5,
        /// <summary>
        /// Saturday as 6
        /// </summary>
        Saturday = 6,
        /// <summary>
        /// Sunday as 7
        /// </summary>
        Sunday = 7
    };

    /// <summary>
    /// Weekday Model as concrete day for a weekplan, with activities
    /// </summary>
    public class Weekday
    {
        /// <summary>
        /// Primary key
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// Day of the week Enum
        /// </summary>
        public Days Day { get; set; }

        /// <summary>
        /// List of activities in the given weekday
        /// </summary>
        public ICollection<Activity> Activities { get; set; }

        /// <summary>
        /// Constructor with including activities
        /// </summary>
        public Weekday(Days day, List<List<Pictogram>> pictograms, List<ActivityState> activityStates) : this()
        {
            if (pictograms.Count != activityStates.Count)
            {
                throw new ArgumentException($"{pictograms.Count} elements are in activityicons, " +
                                            $"but {activityStates.Count} elements are in activityStates. " +
                                            $"The numbers must match.");
            }
            
            this.Day = day;
            for (int i = 0; i < pictograms.Count; i++)
            {
                this.Activities.Add(new Activity(this, pictograms[i], i, activityStates[i]));
            }
        }

        /// <summary>
        /// Constructor for specific WeekdayDTO
        /// </summary>
        /// <param name="day"></param>
        public Weekday(WeekdayDTO day) : this()
        {
            Day = day.Day;
            this.Activities = new List<Activity>();
        }

        /// <summary>
        /// Used for JSON Generation
        /// </summary>
        public Weekday()
        {
            this.Activities = new List<Activity>();
        }
    }
}