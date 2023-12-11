using System.ComponentModel.DataAnnotations;

namespace GirafEntities.WeekPlanner.DTOs
{
    /// <summary>
    /// Defines the structure of image when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class ImageDTO
    {
        /// <summary>
        /// An array of bytes containing the pictogram's image.
        /// </summary>
        [Required]
        public byte[] Image { get; set; }

        /// <summary>
        /// Creates a new image data transfer object from a given base64 string.
        /// </summary>
        /// <param name="image">An array of bytes containing the pictogram's image as a base64 encoded string.</param>
        public ImageDTO(byte[] image)
        {
            this.Image = image;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public ImageDTO() { }
    }
}