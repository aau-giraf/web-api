using System;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of a resource when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class FrameDTO
    {
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
        /// <param name="frame"></param>
        public FrameDTO (Resource frame) {
            this.Id = frame.Id;
            this.LastEdit = frame.LastEdit;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public FrameDTO () {}
    }
}