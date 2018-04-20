using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System;

namespace GirafRest.Models.DTOs
{
    /// <summary>
    /// Defines the structure of Pictogram when serializing and deserializing data. Data transfer objects (DTOs) 
    /// were introduced in the project due to problems with circular references in the model classes.
    /// </summary>
    public class PictogramDTO
    {
        [Required]
        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The id of the pictogram.
        /// </summary>
        /// 
        public long Id { get; set; }
        /// <summary>
        /// The last time the pictogram was edited.
        /// </summary>
        public DateTime LastEdit { get; set; }

        [Required]
        /// <summary>
        /// The accesslevel of the pictogram.
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }

        /// <summary>
        /// An array of bytes containing the pictogram's image.
        /// </summary>
        [JsonIgnore]
        public byte[] Image { get; set; }
        public string ImageUrl {get {return $"/v1/pictogram/{Id}/image/raw";}}
        public string ImageHash {get  { return Image == null ? null : Convert.ToBase64String(MD5.Create().ComputeHash(Image)); }  }

        /// <summary>
        /// Creates a new pictogram data transfer object from a given pictogram.
        /// </summary>
        /// <param name="Pictogram">The pictogram to create a DTO for.</param>
        public PictogramDTO(Pictogram pictogram)
        {
            if (pictogram != null)
            {
                this.Id = pictogram.Id;
                this.LastEdit = pictogram.LastEdit;
                this.Title = pictogram.Title;
                this.AccessLevel = pictogram.AccessLevel;
                this.Image = pictogram.Image;
            }
        }

        /// <summary>
        /// Creates a new pictogram data transfer object from a given pictogram, which also includes the pictogram's image.
        /// </summary>
        /// <param name="pictogram">The pictogram to create a DTO for.</param>
        /// <param name="image">An array of bytes containing the pictogram's image.</param>
        public PictogramDTO(Pictogram pictogram, byte[] image) : this(pictogram)
        {
            this.Image = image;
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public PictogramDTO() {}
    }
}