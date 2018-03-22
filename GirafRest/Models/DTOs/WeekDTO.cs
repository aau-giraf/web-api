using System;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Week when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class WeekDTO
    {
        /// <summary>
        /// The weeks thumbnail.
        /// </summary>
        public PictogramDTO Thumbnail { get; set; }

        /// <summary>
        /// A Name describing the week.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The id of the week.
        /// </summary>
        public long? Id { get; set; }
        /// <summary>
        /// A list of the days in the week schedule.
        /// </summary>
        public ICollection<WeekdayDTO> Days { get; set; }

        /// <summary>
        /// Creates a new data transfer object for a given week.
        /// </summary>
        /// <param name="week">The week to create a DTO for.</param>
        public WeekDTO(Week week)
        {
            this.Name = week.Name;
            try
            {
                this.Thumbnail = new PictogramDTO(week.Thumbnail);
            }
            catch (NullReferenceException)
            {
                this.Thumbnail = new PictogramDTO();
            }
            this.Id = week.Id;
            Days = new List<WeekdayDTO>();
            foreach (var day in week.Weekdays)
            {
                Days.Add(new WeekdayDTO(day));
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR.
        /// </summary>
        public WeekDTO() {}
    }
}