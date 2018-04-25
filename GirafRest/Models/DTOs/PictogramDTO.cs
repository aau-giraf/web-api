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
        public long Id { get; internal set; }

        /// <summary>
        /// The last time the pictogram was edited.
        /// </summary>
        public DateTime LastEdit { get; internal set; }

        /// <summary>
        /// The title of the pictogram.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The accesslevel of the pictogram.
        /// </summary>
        public AccessLevel? AccessLevel { get; set; }

        /// <summary>
        /// An array of bytes containing the pictogram's image.
        /// </summary>
        [JsonIgnore]
        public byte[] Image { get; internal set; }
        public string ImageUrl { get { return $"/v1/pictogram/{Id}/image/raw"; } }
        public string ImageHash { get { return Image == null ? null : Convert.ToBase64String(MD5.Create().ComputeHash(Image)); } }

        public PictogramDTO(Pictogram pictogram)
        {
            if (pictogram != null)
            {
                this.Title = pictogram.Title;
                this.AccessLevel = pictogram.AccessLevel;
                this.Id = pictogram.Id;
                this.LastEdit = pictogram.LastEdit;
                this.Image = pictogram.Image;
            }
        }

        /// <summary>
        /// DO NOT DELETE THIS! NEWTONSOFT REQUIRES AN EMPTY CONSTRUCTOR!
        /// </summary>
        public PictogramDTO()
        {
        }
    }
}