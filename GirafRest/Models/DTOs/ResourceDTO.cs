using System;
using System.ComponentModel.DataAnnotations;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of a resource when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class ResourceDTO
    {
        [Required]
        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// The id of the resource.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// The last time the resource was edited.
        /// </summary>
        public DateTime LastEdit { get; set; }

        /// <summary>
        /// Creates a ResourceDTO from the given resource, fit for sending as a request or response.
        /// </summary>
        /// <param name="frame">The resource in need of transfer</param>
        public ResourceDTO (Resource frame) {
            if (frame != null){
                this.Id = frame.Id;
                this.LastEdit = frame.LastEdit;
            }
        }

        /// <summary>
        /// Empty constructor required by Newtonsoft.
        /// </summary>
        public ResourceDTO () {}
    }
}