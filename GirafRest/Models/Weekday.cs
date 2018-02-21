using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GirafRest.Models.DTOs;

namespace GirafRest.Models
{
    /// <summary>
    /// An enum defining all days of the week.
    /// </summary>
    public enum Days { Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday };
    /// <summary>
    /// A weekday displays what a citizen should do in the course of the day. A weekday may be populated with
    /// a series of Pictograms and choices. They make up the building blocks of Weeks.
    /// </summary>
    public class Weekday
    {
        /// <summary>
        /// The id of the weekday.
        /// </summary>
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        /// <summary>
        /// The last time the weekday was edited.
        /// </summary>
        public DateTime LastEdit { get; set; }
        /// <summary>
        /// A flag indicated whether or not the weekday has been populated.
        /// </summary>
        public bool ElementsSet { get; set; }

        /// <summary>
        /// An enum defining which day of the week this instance represents.
        /// </summary>
        public Days Day { get; set; }

        /// <summary>
        /// A collection of elements that make up the week.
        /// </summary>
        public ICollection<WeekdayResource> Elements { get; set; }

        /// <summary>
        /// DO NOT DELETE THIS.
        /// </summary>
        public Weekday()
        {
            this.Elements = new List<WeekdayResource>();
        }

        /// <summary>
        /// Creates a new weekday.
        /// </summary>
        /// <param name="day">The day of the week which the new weekday should represent.</param>
        /// <param name="elements">A collection of elements that should be added to the weekday.</param>
        public Weekday(Days day, ICollection<Resource> elements)
        {
            this.Day = day;
            this.Elements = new List<WeekdayResource>();
            foreach(var elem in elements) {
                this.Elements.Add(new WeekdayResource(this, (elem)));
            }
        }
        
        /// <summary>
        /// Creates a new weekday from the given WeekdayDTO.
        /// </summary>
        /// <param name="day">A weekday DTO that should be used to create a Weekday from.</param>
        public Weekday(WeekdayDTO day)
        {
            this.Day = day.Day;
            this.Elements = new List<WeekdayResource>();
        }
    }
}