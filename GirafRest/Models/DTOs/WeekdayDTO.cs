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
        /// An enum defining which day of the week this Weekday represents.
        /// </summary>
        public Days Day { get; set; }
        
        /// <summary>
        /// A list of all the elements of the week.
        /// </summary>
        public ICollection<ResourceDTO> Elements { get; set; }
        
        /// <summary>
        /// Creates a new data transfer object for the given week.
        /// </summary>
        /// <param name="weekday">The weekday to create a DTO for.</param>
        public WeekdayDTO(Weekday weekday) {
            this.Day = weekday.Day;
            Elements = new List<ResourceDTO>();
            
            if(weekday.Elements != null){
                foreach (var element in weekday.Elements)
                {
                    if(element.Resource != null){
                        //Be sure to add as right type, not just superclass.
                        if(element.Resource is Pictogram pictogram)
                            Elements.Add(new PictogramDTO(pictogram));
                        
                        //TODO: Fix Loading Choice.Options
                        //else if(element.Resource is Choice choice)
                        //    Elements.Add(new ChoiceDTO(choice));
                    }
                }
            }
        }

        /// <summary>
        /// Empty constructor required for test framework.
        /// </summary>
        public WeekdayDTO() {}
    }
}