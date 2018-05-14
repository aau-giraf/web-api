using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{

    public enum Days { Monday = 1, Tuesday = 2, Wednesday = 3, Thursday = 4, Friday = 5, Saturday = 6, Sunday = 7};

    public class Weekday
    {

        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Days Day { get; set; }

        public ICollection<Activity> Activities { get; set; }

        public Weekday(Days day, List<Pictogram> activityIcons, List<ActivityState> activityStates) : this()
        {
            if (activityIcons.Count != activityStates.Count)
            {
                throw new ArgumentException($"{activityIcons.Count} elements are in activityicons, " +
                                            $"but {activityStates.Count} elements are in activityStates. " +
                                            $"The numbers must match.");
            }
            
            this.Day = day;
            for (int i = 0; i < activityIcons.Count; i++)
            {
                this.Activities.Add(new Activity(this, activityIcons[i], i, activityStates[i]));
            }
        }

        public Weekday(WeekdayDTO day) : this()
        {
            Day = day.Day;
            this.Activities = new List<Activity>();
        }

        /// <summary>
        /// DO NOT DELETE
        /// </summary>
        public Weekday()
        {
            this.Activities = new List<Activity>();
        }
    }
}