using System;
using System.Collections;
using System.Collections.Generic;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Weekday when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class WeekdayDTO
    {
        /// <summary>
        /// The id of the pictogram that is used as the weekday's thumbnail.
        /// </summary>
        public long ThumbnailID { get; set; }
        /// <summary>
        /// Defines whether elements has been added to the weekday or not.
        /// </summary>
        public bool ElementsSet { get; set; }
        /// <summary>
        /// A list of all id's of the resources that make up the weekday.
        /// </summary>
        public List<long> ElementIDs { get; set; }
        /// <summary>
        /// An enum defining which day of the week this Weekday represents.
        /// </summary>
        public Days Day { get; set; }
        /// <summary>
        /// A list of all the elements of the week.
        /// </summary>
        public ICollection<FrameDTO> Elements { get; set; }

        /// <summary>
        /// Creates a new data transfer object for the given week.
        /// </summary>
        /// <param name="weekday">The weekday to create a DTO for.</param>
        public WeekdayDTO(Weekday weekday) {
            try
            {
                this.ThumbnailID = weekday.ThumbnailKey;
            }
            catch (NullReferenceException)
            {
                this.ThumbnailID = 0;
            }
            this.Day = weekday.Day;
            Elements = new List<FrameDTO>();
            ElementIDs = new List<long>();
            if(weekday.Elements != null){
                foreach (var element in weekday.Elements)
                {
                    Elements.Add(new FrameDTO(element.Resource));
                    ElementIDs.Add(element.Resource.Id);
                }
            }
            if(Elements.Count > 0)
                ElementsSet = true;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public WeekdayDTO() {}
    }
}